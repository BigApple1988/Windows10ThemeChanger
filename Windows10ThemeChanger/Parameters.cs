using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Windows10ThemeChanger
{
    internal static class Parameters
    {
        public static bool IsAutoRunEnabled
        {
            get
            {
               
                    System.Reflection.Assembly ass =
                        System.Reflection.Assembly.GetExecutingAssembly();
                    var value = ass.Location;
                    var key = registryKey.GetValue(System.AppDomain.CurrentDomain.FriendlyName);
                return key != null && key.Equals(value);
            }
            set
            {
                if(value==true)
                registryKey.SetValue(System.AppDomain.CurrentDomain.FriendlyName, System.Reflection.Assembly.GetExecutingAssembly().Location);
                else registryKey.DeleteValue(System.AppDomain.CurrentDomain.FriendlyName);
            }
            
        }

        private static RegistryKey registryKey =
            Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");

        

    }
}
