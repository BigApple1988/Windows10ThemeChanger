using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Windows10ThemeChanger
{
    class TrayIcon :ApplicationContext
    {
        #region Private Members

        private System.ComponentModel.IContainer mComponents;
        private NotifyIcon mNotifyIcon;
        private WallpaperCopier wallpaperCopier;
        private string _pathDestination = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\Wallpapers";
        private ContextMenuStrip mContextMenu;
        private ToolStripMenuItem mSwitchAutoRun;
        private ToolStripMenuItem mExitApplication;


        #endregion


        public TrayIcon()
        {
            mComponents = new System.ComponentModel.Container();
            mNotifyIcon = new NotifyIcon(this.mComponents);
            
            mNotifyIcon.Icon = new Icon(GetType(),"Icon.ico");
            mNotifyIcon.Text = "Windows 10 Wallpaper From Lockscreen";
            mNotifyIcon.Visible = true;
            wallpaperCopier = new WallpaperCopier(_pathDestination);
            wallpaperCopier.StartWork();
            mContextMenu = new ContextMenuStrip();
            mSwitchAutoRun = new ToolStripMenuItem();
            mExitApplication = new ToolStripMenuItem();

            //Attach the menu to the notify icon
            mNotifyIcon.ContextMenuStrip = mContextMenu;

            //Setup the items and add them to the menu strip, adding handlers to be created later
            mSwitchAutoRun.Text = AutoRunText();
            mSwitchAutoRun.Click += mSwitchAutoRun_Click;

            mContextMenu.Items.Add(mSwitchAutoRun);
            
            mExitApplication.Text = "Exit";
            mExitApplication.Click += new EventHandler(mExitApplication_Click);
            mContextMenu.Items.Add(mExitApplication);
        }

        private string AutoRunText()
        {
           if(Parameters.IsAutoRunEnabled)
           {
               return "Disable Autorun";
           }
            return "Enable Autorun";


        }

        void mExitApplication_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void mSwitchAutoRun_Click(object sender, EventArgs e)
        {
            Parameters.IsAutoRunEnabled = !Parameters.IsAutoRunEnabled;
            mSwitchAutoRun.Text = AutoRunText();

        }
    }
}
