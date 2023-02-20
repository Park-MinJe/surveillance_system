using System;
using System.Collections.Generic;
using System.Text;

namespace surveillance_system
{
    public partial class Program
    {
        public class GuiManagerForGisBuildingService
        {
            public void setGisMethodNameByGui(string methodName)
            {
                Console.WriteLine(methodName);
                gbs.setGisMethodName(methodName);
            }

            public void setServiceKeyByGui(string serviceKey)
            {
                Console.WriteLine(serviceKey);
                gbs.setServiceKey(serviceKey);
            }

            public void setTypeNameByGui(string typeName)
            {
                Console.WriteLine(typeName);
                gbs.setTypeName(typeName);
            }
            public void setBBoxByGui(string bbox)
            {
                Console.WriteLine(bbox);
                gbs.setBBox(bbox);
            }
            public void setPnuByGui(string pnu)
            {
                Console.WriteLine(pnu);
                gbs.setPnu(pnu);
            }
            public void setMaxFeatureByGui(string maxFeature)
            {
                Console.WriteLine(maxFeature);
                gbs.setMaxFeature(maxFeature);
            }
            public void setResultTypeByGui(string resultType)
            {
                Console.WriteLine(resultType);
                gbs.setResultType(resultType);
            }
            public void setSrsNameByGui(string srsName)
            {
                Console.WriteLine(srsName);
                gbs.setSrsName(srsName);
            }
        }
    }
}
