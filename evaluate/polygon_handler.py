import copy

class PolygonHandler:
  def __init__(self, building_1_poly = None, building_2_poly = None):
    # generated polygon
    self.building_1_poly = copy.deepcopy(building_1_poly)
    # origin osm polygon
    self.building_2_poly = copy.deepcopy(building_2_poly)

  '''
  node: list[x, y]
  building: np.array[[x, y], ...]
  '''
  def dist(self, node1, node2):
    return math.pow(node1[0] - node2[0], 2) + math.pow(node1[1] - node2[1], 2)

  def ccw(self, targetNode, leftNode, rightNode):
    cross_product = (leftNode[0] - targetNode[0])*(rightNode[1] - targetNode[1]) - (rightNode[0] - targetNode[0])*(leftNode[1] - targetNode[1])

    if cross_product > 0: return 1
    elif cross_product < 0: return -1
    else: return 0

  def sortComparator(self, targetNode, leftNode, rightNode):
    direction = self.ccw(targetNode, leftNode, rightNode)
    if direction == 0: return self.dist(targetNode, leftNode) < self.dist(targetNode, rightNode)
    elif direction == 1: return 1
    else: return 0

  def quickSort(self, targetNode, nodeList, lo, hi):
    if hi - lo <= 0:
      return

    pivot = nodeList[int(lo + (hi-lo+1)/2)]
    i = lo
    j = hi

    while i <= j:
      while self.sortComparator(targetNode, nodeList[i], pivot): i += 1
      while self.sortComparator(targetNode, pivot, nodeList[j]): j -= 1
      if i > j: break

      temp = copy.deepcopy(nodeList[i])
      nodeList[i] = copy.deepcopy(nodeList[j])
      nodeList[j] = copy.deepcopy(temp)

      i += 1
      j -= 1

    self.quickSort(targetNode, nodeList, lo, j)
    self.quickSort(targetNode, nodeList, i, hi)

  def lineIntersection(self, l1_p1, l1_p2, l2_p1, l2_p2):
    l1_l2 = self.ccw(l1_p1, l1_p2, l2_p1) * self.ccw(l1_p1, l1_p2, l2_p2)
    l2_l1 = self.ccw(l2_p1, l2_p2, l1_p1) * self.ccw(l2_p1, l2_p2, l1_p2)
    return (l1_l2 < 0) and (l2_l1 < 0)

  def intersectionPoint(self, node1, node2, node3, node4):
    return [
        ((node1[0]*node2[1] - node1[1]*node2[0])*(node3[0] - node4[0]) - (node1[0] - node2[0])*(node3[0]*node4[1] - node3[1]*node4[0]))/((node1[0] - node2[0])*(node3[1]-node4[1]) - (node1[1] - node2[1])*(node3[0] - node4[0])),
        ((node1[0]*node2[1] - node1[1]*node2[0])*(node3[1] - node4[1]) - (node1[1] - node2[1])*(node3[0]*node4[1] - node3[1]*node4[0]))/((node1[0] - node2[0])*(node3[1]-node4[1]) - (node1[1] - node2[1])*(node3[0] - node4[0]))
    ]

  def polygonInOut(self, p, num_vertex, vertices):
    ret = 0

    if vertices[0][0] != vertices[num_vertex][0] or vertices[0][1] != vertices[num_vertex][1]:
      print(vertices)
      print('Last vertex and first vertex are not connected.')
      return -1
    for i in range(0, num_vertex):
      if self.ccw(vertices[i], vertices[i+1], p) == 0:
        min_x = min(vertices[i][0], vertices[i+1][0])
        max_x = max(vertices[i][0], vertices[i+1][0])
        min_y = min(vertices[i][1], vertices[i+1][1])
        max_y = max(vertices[i][1], vertices[i+1][1])

        if min_x <= p[0] and p[0] <= max_x and min_y <= p[1] and p[1] <= max_y:
          return 1

    l1_p1 = [-1, -1]
    l1_p2 = copy.deepcopy(p)
    for i in range(0, num_vertex):
      l2_p1 = copy.deepcopy(vertices[i])
      l2_p2 = copy.deepcopy(vertices[i+1])
      ret += self.lineIntersection(l1_p1, l1_p2, l2_p1, l2_p2)

    ret = ret % 2
    return ret

  def getPolygonArea(self, poly):
    ret = 0
    num_poly_nodes = len(poly)
    #print(poly)

    for j in range(0, num_poly_nodes - 1):
      ret += poly[j][0] * poly[j+1][1] - poly[j+1][0] * poly[j][1]

    ret = -ret if ret < 0 else ret
    ret /= 2.0
    return ret

  def getIntersectionArea(self):
    self.intersection_point = []

    targetNode = self.building_1_poly[0]
    #self.quickSort(targetNode, self.building_1_poly, 1, len(self.building_1_poly)-2)
    targetNode = self.building_2_poly[0]
    #self.quickSort(targetNode, self.building_2_poly, 1, len(self.building_2_poly)-2)

    for i in range(0, len(self.building_1_poly)-1):
      l1_p1 = self.building_1_poly[i]
      l1_p2 = self.building_1_poly[i+1]
      for j in range(0, len(self.building_2_poly)-1):
        l2_p1 = self.building_2_poly[j]
        l2_p2 = self.building_2_poly[j+1]

        if self.lineIntersection(l1_p1, l1_p2, l2_p1, l2_p2):
          self.intersection_point.append(self.intersectionPoint(l1_p1, l1_p2, l2_p1, l2_p2))

    for i in range(0, len(self.building_1_poly)):
      if self.polygonInOut(self.building_1_poly[i], len(self.building_2_poly)-1, self.building_2_poly):
        self.intersection_point.append(self.building_1_poly[i])

    for i in range(0, len(self.building_2_poly)):
      if self.polygonInOut(self.building_2_poly[i], len(self.building_1_poly)-1, self.building_1_poly):
        self.intersection_point.append(self.building_2_poly[i])

    #print(self.building_1_poly)
    #print(self.building_2_poly)
    if len(self.intersection_point) < 3: return 0
    else:
      targetNode = self.intersection_point[0]
      self.quickSort(targetNode, self.intersection_point, 1, len(self.intersection_point)-1)
      return self.getPolygonArea(self.intersection_point)
