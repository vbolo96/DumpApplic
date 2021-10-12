using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumpApplic
{
    public static class Helper
    {

        public static string database = "DumpApplic";// use this declaration in all classes
        public static string CnnValue(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        
            
    
    }
}
