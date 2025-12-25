// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Debug = UnityEngine.Debug;

namespace AmplifyShaderEditor
{
	public sealed class TemplatePostProcessor : AssetPostprocessor
	{
		static void OnPostprocessAllAssets( string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths )
		{
			if ( Application.isBatchMode )
			{
				// @diogo: not necessary in batch mode, just skip it
				return;
			}

			ASEPackageManagerHelper.RequestInfo();
			ASEPackageManagerHelper.Update();

			// leave early if there's no shaders among the imports
			bool containsShaders = importedAssets.Any( a => a.EndsWith( ".shader" ) ) || deletedAssets.Any( a => a.EndsWith( ".shader" ) );
			if ( !containsShaders )
				return;

			bool refreshMenuItems = false;
			for( int i = 0; i < importedAssets.Length; i++ )
			{
				if( TemplateHelperFunctions.CheckIfTemplate( importedAssets[ i ] ) )
				{
					string guid = AssetDatabase.AssetPathToGUID( importedAssets[ i ] );
					TemplateDataParent templateData = TemplatesManager.Instance.GetTemplate( guid );
					if( templateData != null )
					{
						refreshMenuItems = templateData.Reload() || refreshMenuItems;
						int windowCount = IOUtils.AllOpenedWindows.Count;
						AmplifyShaderEditorWindow currWindow = UIUtils.CurrentWindow;
						for( int windowIdx = 0; windowIdx < windowCount; windowIdx++ )
						{
							if( IOUtils.AllOpenedWindows[ windowIdx ].OutsideGraph.CurrentCanvasMode == NodeAvailability.TemplateShader )
							{
								if( IOUtils.AllOpenedWindows[ windowIdx ].OutsideGraph.MultiPassMasterNodes.NodesList[ 0 ].CurrentTemplate == templateData )
								{
									UIUtils.CurrentWindow = IOUtils.AllOpenedWindows[ windowIdx ];
									IOUtils.AllOpenedWindows[ windowIdx ].OutsideGraph.ForceMultiPassMasterNodesRefresh();
								}
							}
						}
						UIUtils.CurrentWindow = currWindow;
					}
					else
					{
						refreshMenuItems = true;
						string name = TemplatesManager.OfficialTemplates.ContainsKey( guid ) ? TemplatesManager.OfficialTemplates[ guid ] : string.Empty;
						TemplateMultiPass mp = TemplateMultiPass.CreateInstance<TemplateMultiPass>();
						mp.Init( name, guid, AssetDatabase.GUIDToAssetPath( guid ), true );
						TemplatesManager.Instance.AddTemplate( mp );
					}
				}
			}

			if( deletedAssets.Length > 0 )
			{
				if( deletedAssets[ 0 ].IndexOf( Constants.InvalidPostProcessDatapath ) < 0 )
				{
					for( int i = 0; i < deletedAssets.Length; i++ )
					{
						string guid = AssetDatabase.AssetPathToGUID( deletedAssets[ i ] );
						TemplateDataParent templateData = TemplatesManager.Instance.GetTemplate( guid );
						if( templateData != null )
						{
							// Close any window using that template
							int windowCount = IOUtils.AllOpenedWindows.Count;
							for( int windowIdx = 0; windowIdx < windowCount; windowIdx++ )
							{
								TemplateMasterNode masterNode = IOUtils.AllOpenedWindows[ windowIdx ].CurrentGraph.CurrentMasterNode as TemplateMasterNode;
								if( masterNode != null && masterNode.CurrentTemplate.GUID.Equals( templateData.GUID ) )
								{
									IOUtils.AllOpenedWindows[ windowIdx ].Close();
								}
							}

							TemplatesManager.Instance.RemoveTemplate( templateData );
							refreshMenuItems = true;
						}
					}
				}
			}

			if( refreshMenuItems )
			{
				refreshMenuItems = false;
				TemplatesManager.Instance.CreateTemplateMenuItems();

				AmplifyShaderEditorWindow currWindow = UIUtils.CurrentWindow;

				int windowCount = IOUtils.AllOpenedWindows.Count;
				for( int windowIdx = 0; windowIdx < windowCount; windowIdx++ )
				{
					UIUtils.CurrentWindow = IOUtils.AllOpenedWindows[ windowIdx ];
					IOUtils.AllOpenedWindows[ windowIdx ].CurrentGraph.ForceCategoryRefresh();
				}
				UIUtils.CurrentWindow = currWindow;
			}

			// reimport menu items at the end of everything, hopefully preventing import loops
			TemplatesManager.Instance.ReimportMenuItems();
		}
	}
}
