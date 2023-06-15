using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace PilotLauncher.PropertyGrid.WPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		private const int ATTACH_PARENT_PROCESS = -1;

		[DllImport("kernel32.dll")] private static extern bool AttachConsole(int dwProcessId);

		public App()
		{
			// Fixes console output in rider
			AttachConsole(ATTACH_PARENT_PROCESS);
		}
	}
}