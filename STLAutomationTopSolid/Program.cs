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
using System.Runtime.CompilerServices;
using TopSolid.Cad.Drafting.Automating;

namespace STLAutomationTopSolid
{
    class Program
    {
        static void Main(string[] args)
		{
			TopSolidHost.Connect();
			TopSolidDesignHost.Connect();
			TopSolidDraftingHost.Connect();

			//TopSolidDesignHost.Connect();

			if (!TopSolidHost.IsConnected)
			{
				string message = "Could not connect to TopSolid";
				MessageBoxButtons buttons = MessageBoxButtons.OK;
				MessageBox.Show(message, "Error", buttons);
				return;
			}
			DocumentId currentDoc = TopSolidHost.Documents.EditedDocument;


			if (!TopSolidDesignHost.IsConnected)
			{
				string message = "Could not connect to TopSolid";
				MessageBoxButtons buttons = MessageBoxButtons.OK;
				MessageBox.Show(message, "Error", buttons);
				return;
			}


			//get current document type



			if (TopSolid.Cad.Design.Automating.TopSolidDesignHost.Parts.IsPart(currentDoc))
			{
				DoStlExport(currentDoc);

			}
			else if (TopSolidDraftingHost.Draftings.IsDrafting(currentDoc))
			{
				DoDxfExport(currentDoc);
			}
			else
			{
				string message = "Current document type is not supporteed";
				MessageBoxButtons buttons = MessageBoxButtons.OK;
				MessageBox.Show(message, "Error", buttons);
				return;
			}



		}

		private static void DoDxfExport(DocumentId currentDoc)
		{
			string currentDocName = GetValidDocName(currentDoc);
			string tempPath = System.IO.Path.GetTempPath();

			int dxfExporterIndex = GetExporterIndex(".dxf");
			if(dxfExporterIndex < 0) 
			{
				return;			
			}

			List<KeyValue> exporterOptions = TopSolidHost.Application.GetExporterOptions(dxfExporterIndex);

			string exportPath = tempPath + currentDocName + ".dxf";


			string configuredPath = ConfigurationManager.AppSettings["LightBurnExePath"];

			TopSolidHost.Documents.Export(dxfExporterIndex, currentDoc, exportPath);

			//open lightburn wih extracted file
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = @configuredPath;
			startInfo.Arguments = exportPath;
			Process.Start(startInfo);

		}

		private static void DoStlExport(DocumentId currentDoc)
		{
			string currentDocName = GetValidDocName(currentDoc);

			string tempPath = System.IO.Path.GetTempPath();


			//search the stl exporter index

			int stlExporterIndex = GetExporterIndex(".stl");

			List<KeyValue> exporterOptions = TopSolidHost.Application.GetExporterOptions(stlExporterIndex);

			string exportPath = tempPath + currentDocName + ".stl";

			TopSolidHost.Documents.Export(stlExporterIndex, currentDoc, exportPath);

			string configuredPath = ConfigurationManager.AppSettings["Chemin"];

			TopSolidHost.Documents.Export(stlExporterIndex, currentDoc, exportPath);


			//open slicer wih extracted file
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = @configuredPath;
			startInfo.Arguments = exportPath;
			Process.Start(startInfo);
		}

		private static int GetExporterIndex(string inExtension)
		{
			int stlExporterIndex = -1;
			for (int i = 0; i < TopSolidHost.Application.ExporterCount; i++)
			{
				TopSolidHost.Application.GetExporterFileType(i, out string fileTypeName, out string[] outFileExtensions);
				if(!outFileExtensions.Contains(inExtension)) { continue; }
				else
				{
					stlExporterIndex = i;
					break;
				}
			}

			return stlExporterIndex;
		}

		private static string GetValidDocName(DocumentId currentDoc)
		{
			string currentDocName = TopSolidHost.Documents.GetName(currentDoc);

			string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

			foreach (char c in invalid)
			{
				currentDocName = currentDocName.Replace(c.ToString(), "");
			}

			currentDocName = currentDocName.Replace(" ", "");
			return currentDocName;
		}
	}
}
