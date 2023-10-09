import numpy as np
import cv2
import matplotlib.pyplot as plt
import math

import pandas as pd

from shapely.geometry import Polygon

class obj_building:
  def __init__(self, polygon, polygon_relative, bound, height):
    self.polygon = polygon
    self.polygon_relative = polygon_relative
    self.bound = bound
    self.height = height

    self.relative_left_lower_corner = np.min(self.polygon_relative[0], axis=0)
    self.relative_right_upper_corner = np.max(self.polygon_relative[0], axis=0)

    self.area = Polygon([(x, y) for x, y in self.polygon_relative[0]]).area

  def get_left_lower_corner(self, other_obj_building):
    return np.min(np.concatenate((np.array([self.relative_left_lower_corner]), np.array([other_obj_building.relative_left_lower_corner])), axis=0), axis=0)

  def get_right_upper_corner(self, other_obj_building):
    return np.max(np.concatenate((np.array([self.relative_left_lower_corner]), np.array([other_obj_building.relative_left_lower_corner])), axis=0), axis=0)

  def get_intersection_points_and_area(self, other_obj_building):
    rt = dict()

    self_polygon = Polygon([(x, y) for x, y in self.polygon_relative[0]])
    other_polygon = Polygon([(x, y) for x, y in other_obj_building.polygon_relative[0]])

    intersection = self_polygon.intersection(other_polygon)

    rt['coords'] = list(intersection.exterior.coords)
    rt['area'] = intersection.area

    return rt

'''
lon(경도) -> x
lat(위도) -> y

소수점 표현 방식 -> DMS 방식 변환
- 도: 소수점 좌표 값의 정수(37)
- 분: 37을 제외한 0.397 X 60 = 23.82 에서 소수점 앞의 정수(23)
- 초: 0.82 X 60 = 49.2 에서 소수점 포함 앞의 4자리(49.2)
출처: https://mchch.tistory.com/230

경도: 1도 = 90180m, 1분 = 1503m,  1초 = 25m
위도: 1도 = 110940m,  1분 = 1849m,  1초 = 30m
출처: https://m.cafe.daum.net/gpsyn/Pllz/530
'''
def relative_point(pmin_lon, pmax_lat, plon, plat, power = 5):
  #x = math.ceil((plon-pmin_lon) * math.pow(10, power))
  #y = math.ceil((pmax_lat-plat) * math.pow(10, power))

  lon_diff = plon-pmin_lon
  lon_d = math.floor(lon_diff)
  lon_m = math.floor((lon_diff - lon_d) * 60)
  lon_s = round(((lon_diff - lon_d) * 60 - lon_m) * 60, 2)
  x = round(lon_d * 90180 + lon_m * 1503 + lon_s * 25)

  lat_diff = pmax_lat-plat
  lat_d = math.floor(lat_diff)
  lat_m = math.floor((lat_diff - lat_d) * 60)
  lat_s = round(((lat_diff - lat_d) * 60 - lat_m) * 60, 2)
  y = round(lat_d * 110940 + lat_m * 1849 + lat_s * 30)

  return [x, y]

def findNodeByWayRef(nodes, ref):
  for node in nodes:
    if node.get('id') == ref:
      return node

def toStringLonLat(plon, plat):
  return "(lon, lat) = (" + str(plon) + ", " + str(plat) + ")"