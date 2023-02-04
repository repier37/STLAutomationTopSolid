using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopSolid.Kernel.Automating;
using TopSolid.Cad.Design.Automating;
using System.Diagnostics;
using System.IO;
using System.Configuration;

namespace STLAutomationTopSolid
{
    class Program
    {
        static void Main(string[] args)
        {
            TopSolidHost.Connect();
            TopSolidDesignHost.Connect();

			//TopSolidDesignHost.Connect();

			if (!TopSolidHost.IsConnected)
				{
					string message = "Could not connect to TopSolid";
					MessageBoxButtons buttons = MessageBoxButtons.OK;
					MessageBox.Show(message, "Error", buttons);
					return;
				}
			DocumentId currentDoc = TopSolidHost.Documents.EditedDocument;


			if(!TopSolidDesignHost.IsConnected)
			{
				string message = "Could not connect to TopSolid";
				MessageBoxButtons buttons = MessageBoxButtons.OK;
				MessageBox.Show(message, "Error", buttons);
				return;
			}

			if (!TopSolid.Cad.Design.Automating.TopSolidDesignHost.Parts.IsPart(currentDoc))
            {
                string message = "Only part can be exported.";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, "Error", buttons);
                return;
            }

			//get shapes in document

			string currentDocName = TopSolidHost.Documents.GetName(currentDoc);

			string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

			foreach (char c in invalid)
			{
				currentDocName = currentDocName.Replace(c.ToString(), "");
			}

			currentDocName = currentDocName.Replace(" ","");




			string tempPath = System.IO.Path.GetTempPath();


			//search the stl exporter index

			int stlExporterIndex = -1;
			for (int i = 0; i < TopSolidHost.Application.ExporterCount; i++)
			{
				TopSolidHost.Application.GetExporterFileType(i, out string fileTypeName, out string[] outFileExtensions);
				if (fileTypeName != "STL" ) { continue; }

				else
				{
					stlExporterIndex = i;
					break;
				}
			}

			

			string exportPath = tempPath + currentDocName +".stl";
            TopSolidHost.Documents.Export(stlExporterIndex, currentDoc, exportPath);
			string configuredPath = ConfigurationManager.AppSettings["Chemin"];

	

			//open slicer wih extracted file
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = @configuredPath;
			startInfo.Arguments = exportPath;
			Process.Start(startInfo);
		}
    }
}
