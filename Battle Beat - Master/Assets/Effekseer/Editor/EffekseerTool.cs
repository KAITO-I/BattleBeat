﻿#if UNITY_EDITOR
#pragma warning disable
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text;
using System.Resources;
using System.Globalization;
using EffekseerTool.Data;
using EffekseerTool.Utl;


namespace EffekseerTool
{
	public class Core
	{
		public const string Version = "1.43b";

		public const string OptionFilePath = "config.option.xml";

		static Data.NodeBase selected_node = null;

		static Data.OptionValues option;

		static Data.PostEffectValues postEffect;

		static Data.EffectBehaviorValues effectBehavior = new Data.EffectBehaviorValues();

		static Data.EffectCullingValues culling = new Data.EffectCullingValues();

		static Data.GlobalValues globalValues = new Data.GlobalValues();

		static int start_frame = 0;

		static int end_frame = 160;

		static bool is_loop = false;

		/*
		public static IViewer Viewer
		{
			get;
			set;
		}
		*/

		public static object MainForm
		{
			get;
			set;
		}

		public static Language Language
		{
			get;
			private set;
		}

		public static Data.NodeRoot Root
		{
			get;
			private set;
		}

		public static string FullPath
		{
			get;
			private set;
		}

		public static bool IsChanged
		{
			get;
			private set;
		}

		/// <summary>
		/// 再生開始フレーム
		/// </summary>
		public static int StartFrame
		{
			get
			{
				return start_frame;
			}
			set
			{
				if (value < 0) return;
				if (start_frame == value) return;

				if (end_frame < value)
				{
					start_frame = end_frame;
				}
				else
				{
					start_frame = value;
				}

				if (OnAfterChangeStartFrame != null)
				{
					OnAfterChangeStartFrame(null, null);
				}
			}
		}

		/// <summary>
		/// 再生終了フレーム
		/// </summary>
		public static int EndFrame
		{
			get
			{
				return end_frame;
			}
			set
			{
				if (value < 0) return;
				if (end_frame == value) return;

				if (start_frame > value)
				{
					end_frame = start_frame;
				}
				else
				{
					end_frame = value;
				}

				if (OnAfterChangeEndFrame != null)
				{
					OnAfterChangeEndFrame(null, null);
				}
			}
		}

		/// <summary>
		/// 再生をループするか?
		/// </summary>
		public static bool IsLoop
		{
			get
			{
				return is_loop;
			}
			set
			{
				if (is_loop == value) return;

				is_loop = value;

				if (OnAfterChangeIsLoop != null)
				{
					OnAfterChangeIsLoop(null, null);
				}
			}
		}
#if SCRIPT_ENABLED
        public static Script.ScriptCollection<Script.CommandScript> CommandScripts
		{
			get;
			private set;
		}

		public static Script.ScriptCollection<Script.SelectedScript> SelectedScripts
		{
			get;
			private set;
		}

		public static Script.ScriptCollection<Script.ExportScript> ExportScripts
		{
			get;
			private set;
		}

		public static Script.ScriptCollection<Script.ImportScript> ImportScripts
		{
			get;
			private set;
		}
#endif
		public static Data.OptionValues Option
		{
			get { return option; }
		}
		
		public static Data.PostEffectValues PostEffect
		{
			get { return postEffect; }
		}

		public static Data.EffectBehaviorValues EffectBehavior
		{
			get { return effectBehavior; }
		}

		public static Data.EffectCullingValues Culling
		{
			get { return culling; }
		}

		public static Data.GlobalValues Global
		{
			get { return globalValues; }
		}


		/// <summary>
		/// 選択中のノード
		/// </summary>
		public static Data.NodeBase SelectedNode
		{
			get
			{
				return selected_node;
			}

			set
			{
				if (selected_node == value) return;

				selected_node = value;
				if (OnAfterSelectNode != null)
				{
					OnAfterSelectNode(null, null);
				}
			}
		}

		/// <summary>
		/// Output message
		/// </summary>
		public static Action<string> OnOutputMessage;

		/// <summary>
		/// Output logs
		/// </summary>
		public static Action<LogLevel, string> OnOutputLog;

		/// <summary>
		/// 選択中のノード変更後イベント
		/// </summary>
		public static event EventHandler OnAfterSelectNode;

		/// <summary>
		/// 新規作成前イベント
		/// </summary>
		public static event EventHandler OnBeforeNew;

		/// <summary>
		/// 新規作成後イベント
		/// </summary>
		public static event EventHandler OnAfterNew;

		/// <summary>
		/// 保存後イベント
		/// </summary>
		public static event EventHandler OnAfterSave;

		/// <summary>
		/// 読込前イベント
		/// </summary>
		public static event EventHandler OnBeforeLoad;

		/// <summary>
		/// 読込後イベント
		/// </summary>
		public static event EventHandler OnAfterLoad;

		/// <summary>
		/// 開始フレーム変更イベント
		/// </summary>
		public static event EventHandler OnAfterChangeStartFrame;

		/// <summary>
		/// 終了フレーム変更イベント
		/// </summary>
		public static event EventHandler OnAfterChangeEndFrame;

		/// <summary>
		/// ループ変更イベント
		/// </summary>
		public static event EventHandler OnAfterChangeIsLoop;

		/// <summary>
		/// 読込後イベント
		/// </summary>
		public static event EventHandler OnReload;

		static Core()
		{
#if SCRIPT_ENABLED
			CommandScripts = new Script.ScriptCollection<Script.CommandScript>();
			SelectedScripts = new Script.ScriptCollection<Script.SelectedScript>();
			ExportScripts = new Script.ScriptCollection<Script.ExportScript>();
			ImportScripts = new Script.ScriptCollection<Script.ImportScript>();
#endif
            // change a separator
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            Language = Language.English;
        }

		public static void Initialize(Language? language = null)
		{
			var entryDirectory = GetEntryDirectory() + "/";

			Command.CommandManager.Changed += new EventHandler(CommandManager_Changed);
			FullPath = string.Empty;

			option = LoadOption(language);

			// Switch the language according to the loaded settings
			Language = Option.GuiLanguage;

			// Switch the culture according to the set language
			switch (Language)
			{
				case Language.English:
					Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
					break;
				case Language.Japanese:
					Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja-JP");
					break;
			}

			New();

			if(!DoExecuteWithMLBundle())
			{
				InitializeScripts(entryDirectory);
			}
		}

		static void InitializeScripts(string entryDirectory)
		{
#if SCRIPT_ENABLED
			// Load scripts
			System.IO.Directory.CreateDirectory(entryDirectory + "scripts");
			System.IO.Directory.CreateDirectory(entryDirectory + "scripts/import");
			System.IO.Directory.CreateDirectory(entryDirectory + "scripts/export");
			System.IO.Directory.CreateDirectory(entryDirectory + "scripts/command");
			System.IO.Directory.CreateDirectory(entryDirectory + "scripts/selected");

			Script.Compiler.Initialize();

			{
				var files = System.IO.Directory.GetFiles(entryDirectory + "scripts/command", "*.*", System.IO.SearchOption.AllDirectories);

				foreach (var file in files)
				{
					var ext = System.IO.Path.GetExtension(file);
					if (ext == ".cs" || ext == ".py")
					{
						Script.CommandScript script = null;
						string error = string.Empty;

						if (Script.Compiler.CompileScript<Script.CommandScript>(file, out script, out error))
						{
							CommandScripts.Add(script);
						}
						else
						{
							if (OnOutputMessage != null)
							{
								OnOutputMessage(error);
							}
						}
					}
				}
			}

			{
				var files = System.IO.Directory.GetFiles(entryDirectory + "scripts/selected", "*.*", System.IO.SearchOption.AllDirectories);
				foreach (var file in files)
				{
					var ext = System.IO.Path.GetExtension(file);
					if (ext == ".cs" || ext == ".py")
					{
						Script.SelectedScript script = null;
						string error = string.Empty;

						if (Script.Compiler.CompileScript<Script.SelectedScript>(file, out script, out error))
						{
							SelectedScripts.Add(script);
						}
						else
						{
							if (OnOutputMessage != null)
							{
								OnOutputMessage(error);
							}
						}
					}
				}
			}

			{
				var files = System.IO.Directory.GetFiles(entryDirectory + "scripts/export", "*.*", System.IO.SearchOption.AllDirectories);
				foreach (var file in files)
				{
					var ext = System.IO.Path.GetExtension(file);
					if (ext == ".cs" || ext == ".py")
					{
						Script.ExportScript script = null;
						string error = string.Empty;

						if (Script.Compiler.CompileScript<Script.ExportScript>(file, out script, out error))
						{
							ExportScripts.Add(script);
						}
						else
						{
							if (OnOutputMessage != null)
							{
								OnOutputMessage(error);
							}
						}

						Console.WriteLine(error);

					}
				}
			}

			{
				var files = System.IO.Directory.GetFiles(entryDirectory + "scripts/import", "*.*", System.IO.SearchOption.AllDirectories);
				foreach (var file in files)
				{
					var ext = System.IO.Path.GetExtension(file);
					if (ext == ".cs" || ext == ".py")
					{
						Script.ImportScript script = null;
						string error = string.Empty;

						if (Script.Compiler.CompileScript<Script.ImportScript>(file, out script, out error))
						{
							ImportScripts.Add(script);
						}
						else
						{
							if (OnOutputMessage != null)
							{
								OnOutputMessage(error);
							}
						}
					}
				}
			}
#endif
		}

		public static void Dispose()
		{
#if SCRIPT_ENABLED
            Script.Compiler.Dispose();
#endif
            SaveOption();
		}

		/// <summary>
		/// Get a directory where the application exists
		/// </summary>
		/// <returns></returns>
		public static string GetEntryDirectory()
		{
			var myAssembly = System.Reflection.Assembly.GetEntryAssembly();
			string path = myAssembly.Location;
			var dir = System.IO.Path.GetDirectoryName(path);

			// for mkbundle
			if (dir == string.Empty)
			{
				dir = System.IO.Path.GetDirectoryName(
				System.IO.Path.GetFullPath(
				Environment.GetCommandLineArgs()[0]));
			}

			return dir;
		}

		public static bool DoExecuteWithMLBundle()
		{
			var myAssembly = System.Reflection.Assembly.GetEntryAssembly();
			string path = myAssembly.Location;
			var dir = System.IO.Path.GetDirectoryName(path);
			return dir == string.Empty;
		}

		public static string Copy(Data.NodeBase node)
		{
			if (node == null) return string.Empty;

			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

			var element = Data.IO.SaveObjectToElement(doc, "CopiedNode", node, true);

			doc.AppendChild(element);

			return doc.InnerXml;
		}

		/// <summary>
		/// Check whether data is valid xml?
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static bool IsValidXml(string data)
		{
			try
			{
				System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
				doc.LoadXml(data);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static void Paste(Data.NodeBase node, string data)
		{
			if (node == null) return;
			if (!IsValidXml(data)) return;

			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

			doc.LoadXml(data);

			if (doc.ChildNodes.Count == 0 || doc.ChildNodes[0].Name != "CopiedNode") return;

			Command.CommandManager.StartCollection();
			Data.IO.LoadFromElement(doc.ChildNodes[0] as System.Xml.XmlElement, node, true);
			Command.CommandManager.EndCollection();
		}

		/// <summary>
		/// New
		/// </summary>
		public static void New()
		{
			if (OnBeforeNew != null)
			{
				OnBeforeNew(null, null);
			}

			StartFrame = 0;
			EndFrame = 120;
			IsLoop = true;

			SelectedNode = null;
			Command.CommandManager.Clear();
			Root = new Data.NodeRoot();

			// Adhoc code
			effectBehavior.Reset();
			culling = new Data.EffectCullingValues();
			globalValues = new Data.GlobalValues();

            // Add a root node
            Root.AddChild();
			Command.CommandManager.Clear();
			FullPath = string.Empty;
			IsChanged = false;

			// Select child
			//SelectedNode = Root.Children[0];

			if (OnAfterNew != null)
			{
				OnAfterNew(null, null);
			}
		}

		public static void SaveTo(string path)
		{
			path = System.IO.Path.GetFullPath(path);

			FullPath = path;

			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

			var element = Data.IO.SaveObjectToElement(doc, "Root", Core.Root, false);

			var behaviorElement = Data.IO.SaveObjectToElement(doc, "Behavior", EffectBehavior, false);
			var cullingElement = Data.IO.SaveObjectToElement(doc, "Culling", Culling, false);
			var globalElement = Data.IO.SaveObjectToElement(doc, "Global", Global, false);

			System.Xml.XmlElement project_root = doc.CreateElement("EffekseerProject");

			project_root.AppendChild(element);

			if(behaviorElement != null) project_root.AppendChild(behaviorElement);
			if (cullingElement != null) project_root.AppendChild(cullingElement);
			if (globalElement != null) project_root.AppendChild(globalElement);

			project_root.AppendChild(doc.CreateTextElement("ToolVersion", Core.Version));
			project_root.AppendChild(doc.CreateTextElement("Version", 3));
			project_root.AppendChild(doc.CreateTextElement("StartFrame", StartFrame));
			project_root.AppendChild(doc.CreateTextElement("EndFrame", EndFrame));
			project_root.AppendChild(doc.CreateTextElement("IsLoop", IsLoop.ToString()));

			doc.AppendChild(project_root);

			var dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
			doc.InsertBefore(dec, project_root);
			doc.Save(path);
			IsChanged = false;

			if (OnAfterSave != null)
			{
				OnAfterSave(null, null);
			}
		}

		public static bool LoadFrom(string path)
		{
			path = System.IO.Path.GetFullPath(path);

			if (!System.IO.File.Exists(path)) return false;
			SelectedNode = null;

			FullPath = path;

			var doc = new System.Xml.XmlDocument();

			doc.Load(path);

			if (doc.ChildNodes.Count != 2) return false;
			if (doc.ChildNodes[1].Name != "EffekseerProject") return false;

			if (OnBeforeLoad != null)
			{
				OnBeforeLoad(null, null);
			}

			uint toolVersion = 0;
			if (doc["EffekseerProject"]["ToolVersion"] != null)
			{
				var fileVersion = doc["EffekseerProject"]["ToolVersion"].GetText();
				var currentVersion = Core.Version;

				toolVersion = ParseVersion(fileVersion);

				if (toolVersion > ParseVersion(currentVersion))
				{
                    switch (Language)
                    {
                        case Language.English:
                            throw new Exception("Version Error : \nThe file is created with a newer version of the tool.\nPlease use the latest version of the tool.");
                            break;
                        case Language.Japanese:
                            throw new Exception("Version Error : \nファイルがより新しいバージョンのツールで作成されています。\n最新バージョンのツールを使用してください。");
                            break;
                    }

                    
				}
			}

            // For compatibility
            {
                // Stripe→Ribbon
                var innerText = doc.InnerXml;
				innerText = innerText.Replace("<Stripe>", "<Ribbon>").Replace("</Stripe>", "</Ribbon>");
				doc = new System.Xml.XmlDocument();
				doc.LoadXml(innerText);
			}

            // For compatibility
            {
                // GenerationTime
                // GenerationTimeOffset

                Action<System.Xml.XmlNode> replace = null;
				replace = (node) =>
					{
						if ((node.Name == "GenerationTime" || node.Name == "GenerationTimeOffset") &&
							node.ChildNodes.Count > 0 &&
							node.ChildNodes[0] is System.Xml.XmlText)
						{
							var name = node.Name;
							var value = node.ChildNodes[0].Value;

							node.RemoveAll();

							var center = doc.CreateElement("Center");
							var max = doc.CreateElement("Max");
							var min = doc.CreateElement("Min");

							center.AppendChild(doc.CreateTextNode(value));
							max.AppendChild(doc.CreateTextNode(value));
							min.AppendChild(doc.CreateTextNode(value));

							node.AppendChild(center);
							node.AppendChild(max);
							node.AppendChild(min);
						}
						else
						{
							for(int i = 0; i < node.ChildNodes.Count; i++)
							{
								replace(node.ChildNodes[i]);
							}
						}
					};

				replace(doc);
			}

			var root = doc["EffekseerProject"]["Root"];
			if (root == null) return false;

			culling = new Data.EffectCullingValues();
			globalValues = new Data.GlobalValues();

			// Adhoc code
			effectBehavior.Reset();

			var behaviorElement = doc["EffekseerProject"]["Behavior"];
			if (behaviorElement != null)
			{
				var o = effectBehavior as object;
				Data.IO.LoadObjectFromElement(behaviorElement as System.Xml.XmlElement, ref o, false);
			}

			var cullingElement = doc["EffekseerProject"]["Culling"];
			if (cullingElement != null)
			{
				var o = culling as object;
				Data.IO.LoadObjectFromElement(cullingElement as System.Xml.XmlElement, ref o, false);
			}

			var globalElement = doc["EffekseerProject"]["Global"];
			if (globalElement != null)
			{
				var o = globalValues as object;
				Data.IO.LoadObjectFromElement(globalElement as System.Xml.XmlElement, ref o, false);
			}

			StartFrame = 0;
			EndFrame = doc["EffekseerProject"]["EndFrame"].GetTextAsInt();
			StartFrame = doc["EffekseerProject"]["StartFrame"].GetTextAsInt();
			IsLoop = bool.Parse(doc["EffekseerProject"]["IsLoop"].GetText());
			IsLoop = true;

			int version = 0;
			if (doc["EffekseerProject"]["Version"] != null)
			{
				version = doc["EffekseerProject"]["Version"].GetTextAsInt();
			}

			var root_node = new Data.NodeRoot() as object;
			Data.IO.LoadObjectFromElement(root as System.Xml.XmlElement, ref root_node, false);

            // For compatibility
            if (version < 3)
			{
				Action<Data.NodeBase> convert = null;
				convert = (n) =>
					{
						var n_ = n as Data.Node;

						if (n_ != null)
						{
							if (n_.DrawingValues.Type.Value == Data.RendererValues.ParamaterType.Sprite)
							{
								n_.RendererCommonValues.ColorTexture.SetAbsolutePathDirectly(n_.DrawingValues.Sprite.ColorTexture.AbsolutePath);
								n_.RendererCommonValues.AlphaBlend.SetValueDirectly(n_.DrawingValues.Sprite.AlphaBlend.Value);
							}
							else if (n_.DrawingValues.Type.Value == Data.RendererValues.ParamaterType.Ring)
							{
								n_.RendererCommonValues.ColorTexture.SetAbsolutePathDirectly(n_.DrawingValues.Ring.ColorTexture.AbsolutePath);
								n_.RendererCommonValues.AlphaBlend.SetValueDirectly(n_.DrawingValues.Ring.AlphaBlend.Value);
							}
							else if (n_.DrawingValues.Type.Value == Data.RendererValues.ParamaterType.Ribbon)
							{
								n_.RendererCommonValues.ColorTexture.SetAbsolutePathDirectly(n_.DrawingValues.Ribbon.ColorTexture.AbsolutePath);
								n_.RendererCommonValues.AlphaBlend.SetValueDirectly(n_.DrawingValues.Ribbon.AlphaBlend.Value);
							}
						}

						for (int i = 0; i < n.Children.Count; i++)
						{
							convert(n.Children[i]);
						}
					};

				convert(root_node as Data.NodeBase);
			}

			Root = root_node as Data.NodeRoot;
			Command.CommandManager.Clear();
			IsChanged = false;

			if (OnAfterLoad != null)
			{
				OnAfterLoad(null, null);
			}

			return true;
		}

		/// <summary>
		/// Load option parameters from config.option.xml
		/// If it failed, return default values.
		/// </summary>
		/// <param name="defaultLanguage"></param>
		/// <returns></returns>
		static public Data.OptionValues LoadOption(Language? defaultLanguage)
		{
            Data.OptionValues res = new Data.OptionValues();
			postEffect = new Data.PostEffectValues();

			var path = System.IO.Path.Combine(GetEntryDirectory(), OptionFilePath);

			if (!System.IO.File.Exists(path))
			{
				if (defaultLanguage != null)
				{
					res.GuiLanguage.SetValueDirectly((Language)defaultLanguage.Value);
				}
				return res;
			}

			var doc = new System.Xml.XmlDocument();

			doc.Load(path);

            if (doc.ChildNodes.Count != 2) return res;
            if (doc.ChildNodes[1].Name != "EffekseerProject") return res;

			var optionElement = doc["EffekseerProject"]["Option"];
			if (optionElement != null)
			{
                var o = res as object;
				Data.IO.LoadObjectFromElement(optionElement as System.Xml.XmlElement, ref o, false);
			}

			var postEffectElement = doc["EffekseerProject"]["PostEffect"];
			if (postEffectElement != null) {
				var o = postEffect as object;
				Data.IO.LoadObjectFromElement(postEffectElement as System.Xml.XmlElement, ref o, false);
			}

			IsChanged = false;

            return res;
		}

		static public void SaveOption()
		{
			var path = System.IO.Path.Combine(GetEntryDirectory(), OptionFilePath);

			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

			var optionElement = Data.IO.SaveObjectToElement(doc, "Option", Option, false);
			var postEffectElement = Data.IO.SaveObjectToElement(doc, "PostEffect", PostEffect, false);

			System.Xml.XmlElement project_root = doc.CreateElement("EffekseerProject");
			if(optionElement != null) project_root.AppendChild(optionElement);
			if(postEffectElement != null) project_root.AppendChild(postEffectElement);

			doc.AppendChild(project_root);

			var dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
			doc.InsertBefore(dec, project_root);
			doc.Save(path);
			IsChanged = false;
		}

		static uint ParseVersion(string versionText)
		{
			versionText = versionText.Replace("CTP1", "");
			versionText = versionText.Replace("CTP2", "");
			versionText = versionText.Replace("CTP3", "");
			versionText = versionText.Replace("CTP4", "");
			versionText = versionText.Replace("CTP5", "");

			versionText = versionText.Replace("α1", "");
			versionText = versionText.Replace("α2", "");
			versionText = versionText.Replace("α3", "");
			versionText = versionText.Replace("α4", "");
			versionText = versionText.Replace("α5", "");

			versionText = versionText.Replace("β1", "");
			versionText = versionText.Replace("β2", "");
			versionText = versionText.Replace("β3", "");
			versionText = versionText.Replace("β4", "");
			versionText = versionText.Replace("β5", "");

			versionText = versionText.Replace("RC1", "");
			versionText = versionText.Replace("RC2", "");
			versionText = versionText.Replace("RC3", "");
			versionText = versionText.Replace("RC4", "");
			versionText = versionText.Replace("RC5", "");

			versionText = versionText.Replace("a", "");
			versionText = versionText.Replace("b", "");
			versionText = versionText.Replace("c", "");
			versionText = versionText.Replace("d", "");
			versionText = versionText.Replace("e", "");
			versionText = versionText.Replace("f", "");
			versionText = versionText.Replace("g", "");
			versionText = versionText.Replace("h", "");
			versionText = versionText.Replace("i", "");

			if (versionText.Length == 2) versionText += "000";
			if (versionText.Length == 3) versionText += "00";
			if (versionText.Length == 4) versionText += "0";

			uint version = 0;
			string[] list = versionText.Split('.');
			int count = Math.Min(list.Length, 4);
			for (int i = 0; i < count; i++) {
				int value = Math.Min(int.Parse(list[i]), 255);
				version |= (uint)value << ((3 - i) * 8);
			}
			return version;
		}

		static void CommandManager_Changed(object sender, EventArgs e)
		{
			IsChanged = true;
		}

		public static void Reload()
		{
			if (OnReload != null)
			{
				OnReload(null, null);
			}
		}

		public static bool MoveNode(Data.Node movedNode, Data.NodeBase targetParent, int targetIndex)
		{
			// Check
			if(movedNode.Parent == targetParent && targetIndex != int.MaxValue)
			{
				var index = targetParent.Children.Internal.Select((i, n) => Tuple35.Create(i, n)).FirstOrDefault(_ => _.Item1 == movedNode).Item2;

				// Not changed.
				if (index == targetIndex || index + 1 == targetIndex)
				{
					return false;
				}
			}

			if(movedNode == targetParent)
			{
				return false;
			}

			Func<Data.Node, bool> isFound = null;
			
			isFound = (Data.Node n) =>
			{
				if(n.Children.Internal.Any(_=>_ == targetParent))
				{
					return true;
				}

				foreach(var n_ in n.Children.Internal)
				{
					if (isFound(n_)) return true;
				}

				return false;
			};

			if(isFound(movedNode))
			{
				return false;
			}

			// 
			if(targetParent == movedNode.Parent && targetIndex != int.MaxValue)
			{
				var index = targetParent.Children.Internal.Select((i, n) => Tuple35.Create(i, n)).FirstOrDefault(_ => _.Item1 == movedNode).Item2;
				if(index < targetIndex)
				{
					targetIndex -= 1;
				}
			}

			Command.CommandManager.StartCollection();
			movedNode.Parent.RemoveChild(movedNode);
			targetParent.AddChild(movedNode, targetIndex);
			Command.CommandManager.EndCollection();
			return true;
		}

		/// <summary>
		/// 現在有効なFカーブを含めたノード情報を取得する。
		/// </summary>
		/// <returns></returns>
		public static Utl.ParameterTreeNode GetFCurveParameterNode()
		{
			// 実行速度を上げるために、全て力技で対応

			// 値を取得する
			Func<Data.Node, Tuple35<string, object>[]> getParameters = (node) =>
				{
					var list = new List<Tuple35<string, object>>();

					if (node.LocationValues.Type.Value == Data.LocationValues.ParamaterType.LocationFCurve)
					{
						var name = "Location";
						if(Language == Language.Japanese)
						{
							name = "位置";
						}

						list.Add(Tuple35.Create(name,(object)node.LocationValues.LocationFCurve.FCurve));
					}

					if (node.RotationValues.Type.Value == Data.RotationValues.ParamaterType.RotationFCurve)
					{
						var name = "Angle";
						if (Language == Language.Japanese)
						{
							name = "角度";
						}

						list.Add(Tuple35.Create(name, (object)node.RotationValues.RotationFCurve.FCurve));
					}

					if (node.ScalingValues.Type.Value == Data.ScaleValues.ParamaterType.FCurve)
					{
						var name = "Scaling Factor";
						if (Language == Language.Japanese)
						{
							name = "拡大率";
						}

						list.Add(Tuple35.Create(name, (object)node.ScalingValues.FCurve.FCurve));
					}

					if (node.RendererCommonValues.UV.Value == Data.RendererCommonValues.UVType.FCurve)
					{
						var name = "UV(Start)";
						if (Language == Language.Japanese)
						{
							name = "UV(始点)";
						}

						list.Add(Tuple35.Create(name, (object)node.RendererCommonValues.UVFCurve.Start));
					}

					if (node.RendererCommonValues.UV.Value == Data.RendererCommonValues.UVType.FCurve)
					{
						var name = "UV(Size)";
						if (Language == Language.Japanese)
						{
							name = "UV(大きさ)";
						}

						list.Add(Tuple35.Create(name, (object)node.RendererCommonValues.UVFCurve.Size));
					}

					if (node.DrawingValues.Type.Value == Data.RendererValues.ParamaterType.Sprite &&
						node.DrawingValues.Sprite.ColorAll.Value == Data.StandardColorType.FCurve)
					{
						var name = "Sprite-Color all(RGBA)";
						if (Language == Language.Japanese)
						{
							name = "スプライト・全体色(RGBA)";
						}

						list.Add(Tuple35.Create(name, (object)node.DrawingValues.Sprite.ColorAll_FCurve.FCurve));
					}

					if (node.DrawingValues.Type.Value == Data.RendererValues.ParamaterType.Model &&
						node.DrawingValues.Model.Color.Value == Data.StandardColorType.FCurve)
					{
						var name = "Model-Color(RGBA)";
						if (Language == Language.Japanese)
						{
							name = "モデル・色(RGBA)";
						}

						list.Add(Tuple35.Create(name, (object)node.DrawingValues.Model.Color_FCurve.FCurve));
					}

					if (node.DrawingValues.Type.Value == Data.RendererValues.ParamaterType.Track)
					{
						if (node.DrawingValues.Track.ColorLeft.Value == Data.StandardColorType.FCurve)
						{
							var name = "Track-Color,Left(RGBA)";
							if (Language == Language.Japanese)
							{
								name = "軌跡・左(RGBA)";
							}

							list.Add(Tuple35.Create(name, (object)node.DrawingValues.Track.ColorLeft_FCurve.FCurve));
						}

						if (node.DrawingValues.Track.ColorLeftMiddle.Value == Data.StandardColorType.FCurve)
						{
							var name = "Track-Color,Left-Center(RGBA)";
							if (Language == Language.Japanese)
							{
								name = "軌跡・左中間(RGBA)";
							}

							list.Add(Tuple35.Create(name, (object)node.DrawingValues.Track.ColorLeftMiddle_FCurve.FCurve));
						}

						if (node.DrawingValues.Track.ColorCenter.Value == Data.StandardColorType.FCurve)
						{
							var name = "Track-Color,Center(RGBA)";
							if (Language == Language.Japanese)
							{
								name = "軌跡・中央(RGBA)";
							}

							list.Add(Tuple35.Create(name, (object)node.DrawingValues.Track.ColorCenter_FCurve.FCurve));
						}

						if (node.DrawingValues.Track.ColorCenterMiddle.Value == Data.StandardColorType.FCurve)
						{
							var name = "Track-Color,Center-Middle(RGBA)";
							if (Language == Language.Japanese)
							{
								name = "軌跡・中央中間(RGBA)";
							}

							list.Add(Tuple35.Create(name, (object)node.DrawingValues.Track.ColorCenterMiddle_FCurve.FCurve));
						}

						if (node.DrawingValues.Track.ColorRight.Value == Data.StandardColorType.FCurve)
						{
							var name = "Track-Color,Right(RGBA)";
							if (Language == Language.Japanese)
							{
								name = "軌跡・右(RGBA)";
							}

							list.Add(Tuple35.Create(name, (object)node.DrawingValues.Track.ColorRight_FCurve.FCurve));
						}

						if (node.DrawingValues.Track.ColorRightMiddle.Value == Data.StandardColorType.FCurve)
						{
							var name = "Track-Color,Right-Center(RGBA)";
							if (Language == Language.Japanese)
							{
								name = "軌跡・右中間(RGBA)";

							}
							list.Add(Tuple35.Create(name, (object)node.DrawingValues.Track.ColorRightMiddle_FCurve.FCurve));
						}
					}

					return list.ToArray();
				};

			// Generate tree
			Func<Data.NodeBase, Utl.ParameterTreeNode> getParameterTreeNodes = null;

			getParameterTreeNodes = (node) =>
				{
					Tuple35<string, object>[] parameters = null;

					var rootNode = node as Data.NodeRoot;
					var normalNode = node as Data.Node;

					if (rootNode != null)
					{
						parameters = new Tuple35<string, object>[0];
					}
					else if (normalNode != null)
					{
						parameters = getParameters(normalNode);
					}

					List<Utl.ParameterTreeNode> children = new List<ParameterTreeNode>();
					for (int i = 0; i < node.Children.Count; i++)
					{
						children.Add(getParameterTreeNodes(node.Children[i]));
					}

					return new ParameterTreeNode(node, parameters, children.ToArray());
				};


			return getParameterTreeNodes(Root);
		}
	}
}

namespace EffekseerTool
{
	/// <summary>
	/// 値が変化したときのイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void ChangedValueEventHandler(object sender, ChangedValueEventArgs e);

	public enum ChangedValueType
	{ 
		Execute,
		Unexecute,
	}

	/// <summary>
	/// 値が変化したときのイベントの引数
	/// </summary>
	public class ChangedValueEventArgs : EventArgs
	{
		public object Value
		{
			get;
			private set;
		}

		public ChangedValueType Type
		{
			get;
			private set;
		}

		internal ChangedValueEventArgs(object value, ChangedValueType type)
		{
			Value = value;
			Type = type;
		}
	}

	/// <summary>
	/// 言語
	/// </summary>
	public enum Language
	{
        [Name(value = "日本語", language = Language.Japanese)]
        [Name(value = "Japanese", language = Language.English)]
		Japanese,
        [Name(value = "英語", language = Language.Japanese)]
        [Name(value = "English", language = Language.English)]
		English,
	}

	public enum LogLevel
	{
		Info,
		Warning,
	}

	/// <summary>
	/// a class for get default language
	/// </summary>
	public class LanguageGetter
	{
		public static Language GetLanguage()
		{
			// Switch the language according to the OS settings
			var culture = System.Globalization.CultureInfo.CurrentCulture;
			if (culture.Name == "ja-JP")
			{
				return Language.Japanese;
			}

			return Language.English;
		}
	}

	// アセンブリからリソースファイルをロードする
	// Resources.GetString(...) に介して取得する場合、
	// カルチャーによってローカライズ済の文字列が得られます。
	public static class Resources
    {
		/* this implementation causes errors in mono
		[DataContract]
		class Data
		{
			[DataMember]
			public Dictionary<string, string> kv;
		}
		*/

		static ResourceManager resources;

		static Dictionary<string, string> keyToStrings = new Dictionary<string, string>();

		static Resources()
        {
        }

		public static void SetResourceManager(ResourceManager resourceManager)
		{
			resources = resourceManager;
		}

		public static void LoadLanguageFile(string path)
		{
			var lines = System.IO.File.ReadAllLines(path);

			foreach(var line in lines)
			{
				var strs = line.Split(',');
				if (strs.Length < 2) continue;

				var key = strs[0];
				var value = string.Join(",", strs.Skip(1).ToArray());
				value = value.Replace(@"\n", "\n");

				keyToStrings.Add(key, value);
			}

			/* this implementation causes errors in mono
			var bytes = System.IO.File.ReadAllBytes(path);
		
			var settings = new DataContractJsonSerializerSettings();
			settings.UseSimpleDictionaryFormat = true;
			var serializer = new DataContractJsonSerializer(typeof(Data), settings);
			{
				var data = (Data)serializer.ReadObject(ms);
				foreach (var x in data.kv)
				{
					keyToStrings = data.kv;
				}
			}
			*/
		}

		public static string GetString(string name)
        {
			if(keyToStrings.ContainsKey(name))
			{
				return keyToStrings[name];
			}

            if (resources == null) return string.Empty;
			
            try
            {
                var value = resources.GetString(name);
                if (!String.IsNullOrEmpty(value)) return value; // 発見した場合、文字列を返す
            }
            catch {}
            
            return string.Empty;
        }
    }

	/// <summary>
	/// 名称を設定する属性
	/// </summary>
	[AttributeUsage(
	AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method,
	AllowMultiple = true,
	Inherited = false)]
	public class NameAttribute : Attribute
	{
        static NameAttribute()
        {
        }

		public NameAttribute()
		{
			language = Language.English;
			value = string.Empty;
		}

		public Language language
		{
			get;
			set;
		}

		public string value
		{
			get;
			set;
		}

		/// <summary>
		/// 属性から名称を取得する。
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public static string GetName(object[] attributes)
		{
			if (attributes != null && attributes.Length > 0)
			{
				foreach (var attribute in attributes)
				{
					if (!(attribute is NameAttribute)) continue;

                    // 先にProperties.Resourcesから検索する
                    var value = Resources.GetString(((NameAttribute)attribute).value);
                    if (!String.IsNullOrEmpty(value)) return value; // 発見した場合、文字列を返す

                    // なければ、既存振る舞いで返す
					if (((NameAttribute)attribute).language == Core.Language)
					{
						return ((NameAttribute)attribute).value;
					}
				}
			}

			return string.Empty;
		}
	}

	/// <summary>
	/// 説明を設定する属性
	/// </summary>
	[AttributeUsage(
	AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field,
	AllowMultiple = true,
	Inherited = false)]
	public class DescriptionAttribute : Attribute
	{
		public DescriptionAttribute()
		{
			language = Language.English;
			value = string.Empty;
		}

		public Language language
		{
			get;
			set;
		}

		public string value
		{
			get;
			set;
		}

		/// <summary>
		/// 属性から説明を取得する。
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public static string GetDescription(object[] attributes)
		{
			if (attributes != null && attributes.Length > 0)
			{
				foreach (var attribute in attributes)
				{
					if (!(attribute is DescriptionAttribute)) continue;

                    // 先にProperties.Resourcesから検索する
                    var value = Resources.GetString(((DescriptionAttribute)attribute).value);
                    if (!String.IsNullOrEmpty(value)) return value; // 発見した場合、文字列を返す

					if (((DescriptionAttribute)attribute).language == Core.Language)
					{
						return ((DescriptionAttribute)attribute).value;
					}
				}
			}

			return string.Empty;
		}
	}

	
	/// <summary>
	/// アイコンを設定する属性
	/// </summary>
	[AttributeUsage(
	AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method,
	AllowMultiple = true,
	Inherited = false)]
	public class IconAttribute : Attribute
	{
		public IconAttribute()
		{
			resourceName = string.Empty;
		}

		public string resourceName
		{
			get;
			set;
		}
		
		/// <summary>
		/// アイコン属性を探す。
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public static IconAttribute GetIcon(object[] attributes)
		{
			if (attributes != null && attributes.Length > 0)
			{
				foreach (var attribute in attributes)
				{
					if (!(attribute is IconAttribute)) continue;

					return (IconAttribute)attribute;
				}
			}

			return null;
		}
	}

	public class Setting
	{
		public static System.Globalization.NumberFormatInfo NFI
		{
			get;
			private set;
		}

		static Setting()
		{
			var culture = new System.Globalization.CultureInfo("ja-JP");
			NFI = culture.NumberFormat;
		}
	}

    public class Tuple35<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;

        public Tuple35()
        {
            Item1 = default(T1);
            Item2 = default(T2);
        }

        public Tuple35(T1 t1, T2 t2)
        {
            Item1 = t1;
            Item2 = t2;
        }
    }

    public class Tuple35
    {
        public static Tuple35<TV1, TV2> Create<TV1, TV2>(TV1 t1, TV2 t2)
        {
            return new Tuple35<TV1, TV2>(t1, t2);
        }
    }
}

namespace EffekseerTool.Binary
{
	[StructLayout(LayoutKind.Sequential)]
	struct CommonValues
	{
		public int MaxCreation;
		public int TranslationEffectType;
		public int RotationEffectType;
		public int ScalingEffectType;
		public int RemoveWhenLifeIsExtinct;
		public int RemoveWhenParentIsRemoved;
		public int RemoveWhenChildrenIsExtinct;
		public int Life_Max;
		public int Life_Min;
		public float CreationTime_Max;
		public float CreationTime_Min;
		public float CreationTimeOffset_Max;
		public float CreationTimeOffset_Min;

		public static byte[] GetBytes(Data.CommonValues value)
		{
			List<byte[]> data = new List<byte[]>();

			var bytes = CommonValues.Create(value).GetBytes();
			data.Add(bytes.Count().GetBytes());
			data.Add(bytes);

			return data.ToArray().ToArray();
		}

		static public CommonValues Create(Data.CommonValues value)
		{
			var s_value = new CommonValues();

			s_value.MaxCreation = value.MaxGeneration.Value;
			if (value.MaxGeneration.Infinite)
			{
				s_value.MaxCreation = int.MaxValue;
			}

			s_value.TranslationEffectType = value.LocationEffectType.GetValueAsInt();
			s_value.RotationEffectType = value.RotationEffectType.GetValueAsInt();
			s_value.ScalingEffectType = value.ScaleEffectType.GetValueAsInt();

			if (value.RemoveWhenLifeIsExtinct)
			{
				s_value.RemoveWhenLifeIsExtinct = 1;
			}
			else
			{
				s_value.RemoveWhenLifeIsExtinct = 0;
			}

			if (value.RemoveWhenParentIsRemoved)
			{
				s_value.RemoveWhenParentIsRemoved = 1;
			}
			else
			{
				s_value.RemoveWhenParentIsRemoved = 0;
			}

			if (value.RemoveWhenAllChildrenAreRemoved)
			{
				s_value.RemoveWhenChildrenIsExtinct = 1;
			}
			else
			{
				s_value.RemoveWhenChildrenIsExtinct = 0;
			}

			s_value.Life_Max = value.Life.Max;
			s_value.Life_Min = value.Life.Min;
			s_value.CreationTime_Max = value.GenerationTime.Max;
			s_value.CreationTime_Min = value.GenerationTime.Min;
			s_value.CreationTimeOffset_Max = value.GenerationTimeOffset.Max;
			s_value.CreationTimeOffset_Min = value.GenerationTimeOffset.Min;

			return s_value;
		}
	}
}

namespace EffekseerTool.Binary
{
	enum NodeType : int
	{ 
		Root = -1,
		None = 0,
		Sprite = 2,
		Ribbon = 3,
        Ring = 4,
		Model = 5,
		Track = 6,
	};

	[StructLayout(LayoutKind.Sequential)]
	struct Vector3D
	{
		float X;
		float Y;
		float Z;

		public Vector3D(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}

	class Utils
	{
		public static void LogFileNotFound(string path)
		{
			if (Core.OnOutputLog != null)
			{
				Language language = Language.English;

				if (Core.Option != null && Core.Option.GuiLanguage != null)
				{
					language = Core.Option.GuiLanguage.Value;
				}
				else
				{
					language = LanguageGetter.GetLanguage();
				}

				if (language == Language.Japanese)
				{
					Core.OnOutputLog(LogLevel.Warning, path + " が見つかりません。");
				}
				else
				{
					Core.OnOutputLog(LogLevel.Warning, path + " is not found.");
				}
			}
		}
	}
}

namespace EffekseerTool.Binary
{
	public class Exporter
	{
		const int Version = 13;

		public HashSet<string> UsedTextures = new HashSet<string>();

		public HashSet<string> UsedNormalTextures = new HashSet<string>();

		public HashSet<string> UsedDistortionTextures = new HashSet<string>();

		/// <summary>
		/// エフェクトデータの出力
		/// </summary>
		/// <returns></returns>
		public byte[] Export(float magnification = 1.0f)
		{
			List<byte[]> data = new List<byte[]>();

			// ヘッダ
			data.Add(Encoding.UTF8.GetBytes("SKFE"));

			// バージョン
			data.Add(BitConverter.GetBytes(Version));

			// reset texture names
            UsedTextures = new HashSet<string>();

			UsedNormalTextures = new HashSet<string>();

			UsedDistortionTextures = new HashSet<string>();

            // ウェーブ名称一覧取得
            HashSet<string> waves = new HashSet<string>();

			// モデル名称一覧取得
			HashSet<string> models = new HashSet<string>();

			Action<Data.NodeBase> get_textures = null;
			get_textures = (node) =>
				{
					if (node is Data.Node)
					{
						var _node = node as Data.Node;

						if (!_node.IsRendered)
						{
						}
						else
						{
							{
								var relative_path = _node.RendererCommonValues.ColorTexture.RelativePath;
								if (relative_path != string.Empty)
								{
									if(_node.RendererCommonValues.Distortion.Value)
									{
										if (!UsedDistortionTextures.Contains(relative_path))
										{
											UsedDistortionTextures.Add(relative_path);
										}
									}
									else
									{
										if (!UsedTextures.Contains(relative_path))
										{
											UsedTextures.Add(relative_path);
										}
									}
								}
							}

							{
								var relative_path = _node.DrawingValues.Model.NormalTexture.RelativePath;
								if (relative_path != string.Empty)
								{
									if (!UsedNormalTextures.Contains(relative_path))
									{
										UsedNormalTextures.Add(relative_path);
									}
								}
							}
						}
					}

					for (int i = 0; i < node.Children.Count; i++)
					{
						get_textures(node.Children[i]);
					}
				};

			get_textures(Core.Root);

            Dictionary<string, int> texture_and_index = new Dictionary<string, int>();
            {
                int index = 0;
                foreach (var texture in UsedTextures.ToList().OrderBy(_ => _))
                {
                    texture_and_index.Add(texture, index);
                    index++;
                }
            }

			Dictionary<string, int> normalTexture_and_index = new Dictionary<string, int>();
			{
				int index = 0;
				foreach (var texture in UsedNormalTextures.ToList().OrderBy(_ => _))
				{
					normalTexture_and_index.Add(texture, index);
					index++;
				}
			}

			Dictionary<string, int> distortionTexture_and_index = new Dictionary<string, int>();
			{
				int index = 0;
				foreach (var texture in UsedDistortionTextures.ToList().OrderBy(_ => _))
				{
					distortionTexture_and_index.Add(texture, index);
					index++;
				}
			}

            Action<Data.NodeBase> get_waves = null;
            get_waves = (node) =>
            {
                if (node is Data.Node)
                {
                    var _node = node as Data.Node;

                    if (_node.SoundValues.Type.GetValue() == Data.SoundValues.ParamaterType.None)
                    {
                    }
                    else if (_node.SoundValues.Type.GetValue() == Data.SoundValues.ParamaterType.Use)
                    {
                        var relative_path = _node.SoundValues.Sound.Wave.RelativePath;
                        if (relative_path != string.Empty)
                        {
                            if (!waves.Contains(relative_path))
                            {
                                waves.Add(relative_path);
                            }
                        }
                    }
                }

                for (int i = 0; i < node.Children.Count; i++)
                {
                    get_waves(node.Children[i]);
                }
            };

            get_waves(Core.Root);

            Dictionary<string, int> wave_and_index = new Dictionary<string, int>();
            {
                int index = 0;
                foreach (var wave in waves.ToList().OrderBy(_ => _))
                {
                    wave_and_index.Add(wave, index);
                    index++;
                }
            }

			Action<Data.NodeBase> get_models = null;
			get_models = (node) =>
			{
				if (node is Data.Node)
				{
					var _node = node as Data.Node;

					if (_node.IsRendered && _node.DrawingValues.Type.Value == Data.RendererValues.ParamaterType.Model)
					{
						var relative_path = _node.DrawingValues.Model.Model.RelativePath;

                        if (!string.IsNullOrEmpty(relative_path))
                        {
							if(string.IsNullOrEmpty(System.IO.Path.GetDirectoryName(relative_path)))
							{
								relative_path = System.IO.Path.GetFileNameWithoutExtension(relative_path) + ".efkmodel";
							}
							else
							{
								relative_path = System.IO.Path.GetDirectoryName(relative_path) + "/" + System.IO.Path.GetFileNameWithoutExtension(relative_path) + ".efkmodel";
							}

							if (relative_path != string.Empty)
                            {
                                if (!models.Contains(relative_path))
                                {
                                    models.Add(relative_path);
                                }
                            }
                        }
                       
					}

					if (_node.GenerationLocationValues.Type.Value == Data.GenerationLocationValues.ParameterType.Model)
					{
						var relative_path = _node.GenerationLocationValues.Model.Model.RelativePath;

                        if (!string.IsNullOrEmpty(relative_path))
                        {
							if (string.IsNullOrEmpty(System.IO.Path.GetDirectoryName(relative_path)))
							{
								relative_path = System.IO.Path.GetFileNameWithoutExtension(relative_path) + ".efkmodel";
							}
							else
							{
								relative_path = System.IO.Path.GetDirectoryName(relative_path) + "/" + System.IO.Path.GetFileNameWithoutExtension(relative_path) + ".efkmodel";
							}

							if (relative_path != string.Empty)
                            {
                                if (!models.Contains(relative_path))
                                {
                                    models.Add(relative_path);
                                }
                            }
                        }
					}
				}

				for (int i = 0; i < node.Children.Count; i++)
				{
					get_models(node.Children[i]);
				}
			};

			get_models(Core.Root);

			Dictionary<string, int> model_and_index = new Dictionary<string, int>();
			{
				int index = 0;
				foreach (var model in models.ToList().OrderBy(_ => _))
				{
					model_and_index.Add(model, index);
					index++;
				}
			}

			// get all nodes
			var nodes = new List<Data.Node>();

			Action<Data.NodeBase> get_nodes = null;
			get_nodes = (node) =>
			{
				if (node is Data.Node)
				{
					var _node = node as Data.Node;
					nodes.Add(_node);
				}

				for (int i = 0; i < node.Children.Count; i++)
				{
					get_nodes(node.Children[i]);
				}
			};

			get_nodes(Core.Root);

			var snode2ind = nodes.
				Select((v, i) => Tuple35.Create(v, i)).
				OrderBy(_ => _.Item1.DepthValues.DrawingPriority.Value * 255 + _.Item2).
				Select((v, i) => Tuple35.Create(v.Item1, i)).ToList();

				// ファイルにテクスチャ一覧出力
				data.Add(BitConverter.GetBytes(texture_and_index.Count));
			foreach (var texture in texture_and_index)
			{
				var path = Encoding.Unicode.GetBytes(texture.Key);
				data.Add(((path.Count() + 2) / 2).GetBytes());
				data.Add(path);
				data.Add(new byte[] { 0, 0 });
			}

			data.Add(BitConverter.GetBytes(normalTexture_and_index.Count));
			foreach (var texture in normalTexture_and_index)
			{
				var path = Encoding.Unicode.GetBytes(texture.Key);
				data.Add(((path.Count() + 2) / 2).GetBytes());
				data.Add(path);
				data.Add(new byte[] { 0, 0 });
			}

			data.Add(BitConverter.GetBytes(distortionTexture_and_index.Count));
			foreach (var texture in distortionTexture_and_index)
			{
				var path = Encoding.Unicode.GetBytes(texture.Key);
				data.Add(((path.Count() + 2) / 2).GetBytes());
				data.Add(path);
				data.Add(new byte[] { 0, 0 });
			}

            // ファイルにウェーブ一覧出力
            data.Add(BitConverter.GetBytes(wave_and_index.Count));
            foreach (var wave in wave_and_index)
            {
                var path = Encoding.Unicode.GetBytes(wave.Key);
                data.Add(((path.Count() + 2) / 2).GetBytes());
                data.Add(path);
                data.Add(new byte[] { 0, 0 });
            }

			// ファイルにモデル一覧出力
			data.Add(BitConverter.GetBytes(model_and_index.Count));
			foreach (var model in model_and_index)
			{
				var path = Encoding.Unicode.GetBytes(model.Key);
				data.Add(((path.Count() + 2) / 2).GetBytes());
				data.Add(path);
				data.Add(new byte[] { 0, 0 });
			}

			// Export the number of nodes
			data.Add(BitConverter.GetBytes(snode2ind.Count));

			var renderPriorityThreshold = snode2ind.Where(_ => _.Item1.DepthValues.DrawingPriority.Value < 0).Count();
			data.Add(BitConverter.GetBytes(renderPriorityThreshold));

			// Export magnification
			data.Add(BitConverter.GetBytes(magnification));

			// Export default seed
			int randomSeed = Core.Global.RandomSeed.Value;
			data.Add(BitConverter.GetBytes(randomSeed));

			// カリングを出力
			data.Add(BitConverter.GetBytes((int)Core.Culling.Type.Value));

			if (Core.Culling.Type.Value == Data.EffectCullingValues.ParamaterType.Sphere)
			{
				data.Add(BitConverter.GetBytes(Core.Culling.Sphere.Radius));
				data.Add(BitConverter.GetBytes(Core.Culling.Sphere.Location.X));
				data.Add(BitConverter.GetBytes(Core.Culling.Sphere.Location.Y));
				data.Add(BitConverter.GetBytes(Core.Culling.Sphere.Location.Z));
			}
			
			// ノード情報出力
			Action<Data.NodeRoot> outout_rootnode = null;
			Action<Data.Node> outout_node = null;

			outout_rootnode = (n) =>
				{
					data.Add(((int)NodeType.Root).GetBytes());
					data.Add(n.Children.Count.GetBytes());
					for (int i = 0; i < n.Children.Count; i++)
					{
						outout_node(n.Children[i]);
					}
				};

			outout_node = (n) =>
			{
				List<byte[]> node_data = new List<byte[]>();

				var isRenderParamExported = n.IsRendered.GetValue();

				for (int i = 0; i < n.Children.Count; i++)
				{
					var nc = n.Children[i];
					var v = nc.RendererCommonValues.ColorInheritType.GetValue();
					if (v == Data.ParentEffectType.Already || v == Data.ParentEffectType.WhenCreating)
					{
						isRenderParamExported = true;
						break;
					}
				}

				if (!isRenderParamExported)
				{
					data.Add(((int)NodeType.None).GetBytes());
				}
				else if (n.DrawingValues.Type.Value == Data.RendererValues.ParamaterType.None)
				{
					data.Add(((int)NodeType.None).GetBytes());
				}
				else if (n.DrawingValues.Type.Value == Data.RendererValues.ParamaterType.Sprite)
				{
					data.Add(((int)NodeType.Sprite).GetBytes());
				}
				else if (n.DrawingValues.Type.Value == Data.RendererValues.ParamaterType.Ribbon)
				{
					data.Add(((int)NodeType.Ribbon).GetBytes());
				}
				else if (n.DrawingValues.Type.Value == Data.RendererValues.ParamaterType.Ring)
				{
					data.Add(((int)NodeType.Ring).GetBytes());
				}
				else if (n.DrawingValues.Type.Value == Data.RendererValues.ParamaterType.Model)
				{
					data.Add(((int)NodeType.Model).GetBytes());
				}
				else if (n.DrawingValues.Type.Value == Data.RendererValues.ParamaterType.Track)
				{
					data.Add(((int)NodeType.Track).GetBytes());
				}
				else
				{
					throw new Exception();
				}

				// Whether to draw the node.
				if (n.IsRendered)
				{
					int v = 1;
					node_data.Add(BitConverter.GetBytes(v));
				}
				else
				{
					int v = 0;
					node_data.Add(BitConverter.GetBytes(v));
				}

				// render order
				{
					var s = snode2ind.FirstOrDefault(_ => _.Item1 == n);
					if(s.Item1 != null)
					{
						node_data.Add(BitConverter.GetBytes(s.Item2));
					}
					else
					{
						node_data.Add(BitConverter.GetBytes(-1));
					}
				}

				node_data.Add(CommonValues.GetBytes(n.CommonValues));
				node_data.Add(LocationValues.GetBytes(n.LocationValues, n.CommonValues.ScaleEffectType));
				node_data.Add(LocationAbsValues.GetBytes(n.LocationAbsValues, n.CommonValues.ScaleEffectType));
				node_data.Add(RotationValues.GetBytes(n.RotationValues));
				node_data.Add(ScaleValues.GetBytes(n.ScalingValues, n.CommonValues.ScaleEffectType));
				node_data.Add(GenerationLocationValues.GetBytes(n.GenerationLocationValues, n.CommonValues.ScaleEffectType, model_and_index));

				// Export depth
                node_data.Add(n.DepthValues.DepthOffset.Value.GetBytes());
				node_data.Add(BitConverter.GetBytes(n.DepthValues.IsScaleChangedDependingOnDepthOffset.Value ? 1 : 0));
				node_data.Add(BitConverter.GetBytes(n.DepthValues.IsDepthOffsetChangedDependingOnParticleScale.Value ? 1 : 0));
				node_data.Add(((int)n.DepthValues.ZSort.Value).GetBytes());
				node_data.Add(n.DepthValues.DrawingPriority.Value.GetBytes());
				node_data.Add(n.DepthValues.SoftParticle.Value.GetBytes());

                node_data.Add(RendererCommonValues.GetBytes(n.RendererCommonValues, texture_and_index, distortionTexture_and_index));

				if (isRenderParamExported)
				{
					node_data.Add(RendererValues.GetBytes(n.DrawingValues, texture_and_index, normalTexture_and_index, model_and_index));
				}
				else
				{
					node_data.Add(RendererValues.GetBytes(null, texture_and_index, normalTexture_and_index, model_and_index));
				}

				data.Add(node_data.ToArray().ToArray());

				data.Add(SoundValues.GetBytes(n.SoundValues, wave_and_index));

				data.Add(n.Children.Count.GetBytes());
				for (int i = 0; i < n.Children.Count; i++)
				{
					outout_node(n.Children[i]);
				}
			};

			outout_rootnode(Core.Root);

			return data.ToArray().ToArray();
		}
	}
}

namespace EffekseerTool.Binary
{
	class GenerationLocationValues
	{
		public static byte[] GetBytes(Data.GenerationLocationValues value, Data.ParentEffectType parentEffectType, Dictionary<string, int> model_and_index)
		{
			List<byte[]> data = new List<byte[]>();
			if (value.EffectsRotation)
			{
				data.Add((1).GetBytes());
			}
			else
			{
				data.Add((0).GetBytes());
			}

			data.Add(value.Type.GetValueAsInt().GetBytes());

			if (value.Type.GetValue() == Data.GenerationLocationValues.ParameterType.Point)
			{
				data.Add(value.Point.Location.GetBytes(1.0f));
			}
			else if (value.Type.GetValue() == Data.GenerationLocationValues.ParameterType.Sphere)
			{
				data.Add((value.Sphere.Radius.Max).GetBytes());
				data.Add((value.Sphere.Radius.Min).GetBytes());
				data.Add((value.Sphere.RotationX.Max / 180.0f * (float)Math.PI).GetBytes());
				data.Add((value.Sphere.RotationX.Min / 180.0f * (float)Math.PI).GetBytes());
				data.Add((value.Sphere.RotationY.Max / 180.0f * (float)Math.PI).GetBytes());
				data.Add((value.Sphere.RotationY.Min / 180.0f * (float)Math.PI).GetBytes());
			}
			if (value.Type.GetValue() == Data.GenerationLocationValues.ParameterType.Model)
			{
				var relative_path = value.Model.Model.RelativePath;

				if(!string.IsNullOrEmpty(relative_path))
				{
					if (string.IsNullOrEmpty(System.IO.Path.GetDirectoryName(relative_path)))
					{
						relative_path = System.IO.Path.GetFileNameWithoutExtension(relative_path) + ".efkmodel";
					}
					else
					{
						relative_path = System.IO.Path.GetDirectoryName(relative_path) + "/" + System.IO.Path.GetFileNameWithoutExtension(relative_path) + ".efkmodel";
					}

					if (model_and_index.ContainsKey(relative_path))
					{
						data.Add(model_and_index[relative_path].GetBytes());
					}
					else
					{
						data.Add(((int)-1).GetBytes());
					}
				}
				else
				{
					data.Add(((int)-1).GetBytes());
				}

				data.Add( ((int)value.Model.Type.Value).GetBytes());
			}
			else if (value.Type.GetValue() == Data.GenerationLocationValues.ParameterType.Circle)
			{
				data.Add((value.Circle.Division.Value).GetBytes());
				data.Add((value.Circle.Radius.Max).GetBytes());
				data.Add((value.Circle.Radius.Min).GetBytes());
				data.Add((value.Circle.AngleStart.Max / 180.0f * (float)Math.PI).GetBytes());
				data.Add((value.Circle.AngleStart.Min / 180.0f * (float)Math.PI).GetBytes());
				data.Add((value.Circle.AngleEnd.Max / 180.0f * (float)Math.PI).GetBytes());
				data.Add((value.Circle.AngleEnd.Min / 180.0f * (float)Math.PI).GetBytes());
				data.Add(((int)value.Circle.Type.Value).GetBytes());

				// Version 1.30(10)
				data.Add(((int)value.Circle.AxisDirection.Value).GetBytes());

				data.Add((value.Circle.AngleNoize.Max / 180.0f * (float)Math.PI).GetBytes());
				data.Add((value.Circle.AngleNoize.Min / 180.0f * (float)Math.PI).GetBytes());
			}

			else if (value.Type.GetValue() == Data.GenerationLocationValues.ParameterType.Line)
			{
				data.Add((value.Line.Division.Value).GetBytes());
				data.Add(value.Line.PositionStart.GetBytes(1.0f));
				data.Add(value.Line.PositionEnd.GetBytes(1.0f));
				data.Add((value.Line.PositionNoize.Max).GetBytes());
				data.Add((value.Line.PositionNoize.Min).GetBytes());
				data.Add(((int)value.Line.Type.Value).GetBytes());
			}

			return data.ToArray().ToArray();
		}
	}
}

namespace EffekseerTool.Binary
{
	class LocationAbsValues
	{
		public static byte[] GetBytes(Data.LocationAbsValues value, Data.ParentEffectType parentEffectType)
		{
			List<byte[]> data = new List<byte[]>();
			data.Add(value.Type.GetValueAsInt().GetBytes());

			if (value.Type.GetValue() == Data.LocationAbsValues.ParamaterType.Gravity)
			{
				var bytes = TranslationAbs_Gravity_Values.Create(value.Gravity).GetBytes();
				data.Add(bytes.Count().GetBytes());
				data.Add(bytes);
			}
			else if (value.Type.GetValue() == Data.LocationAbsValues.ParamaterType.AttractiveForce)
			{
				var bytes = TranslationAbs_AttractiveForce_Values.Create(value.AttractiveForce).GetBytes();
				data.Add(bytes.Count().GetBytes());
				data.Add(bytes);
			}
			else if (value.Type.GetValue() == Data.LocationAbsValues.ParamaterType.None)
			{
				data.Add(((int)0).GetBytes());
			}

			return data.ToArray().ToArray();
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	struct TranslationAbs_Gravity_Values
	{
		public Vector3D Gravity;

		static public TranslationAbs_Gravity_Values Create(Data.LocationAbsValues.GravityParamater value)
		{
			var s_value = new TranslationAbs_Gravity_Values();

			s_value.Gravity = new Vector3D(
				value.Gravity.X,
				value.Gravity.Y,
				value.Gravity.Z);

			return s_value;
		}
	}
	
	[StructLayout(LayoutKind.Sequential)]
	struct TranslationAbs_AttractiveForce_Values
	{
		public float Force;
		public float Control;
		public float MinRange;
		public float MaxRange;

		static public TranslationAbs_AttractiveForce_Values Create(Data.LocationAbsValues.AttractiveForceParamater value)
		{
			var s_value = new TranslationAbs_AttractiveForce_Values();

			s_value.Force = value.Force.Value;
			s_value.Control = value.Control.Value;
			s_value.MinRange = value.MinRange.Value;
			s_value.MaxRange = value.MaxRange.Value;

			return s_value;
		}
	}
}

namespace EffekseerTool.Binary
{
	class LocationValues
	{
		public static byte[] GetBytes(Data.LocationValues value, Data.ParentEffectType parentEffectType)
		{
			//if (parentEffectType != Data.ParentEffectType.NotBind) magnification = 1.0f;

			List<byte[]> data = new List<byte[]>();
			data.Add(value.Type.GetValueAsInt().GetBytes());

			if (value.Type.GetValue() == Data.LocationValues.ParamaterType.Fixed)
			{
				var bytes = Translation_Fixed_Values.Create(value.Fixed, 1.0f).GetBytes();
				data.Add(bytes.Count().GetBytes());
				data.Add(bytes);
			}
			else if (value.Type.GetValue() == Data.LocationValues.ParamaterType.PVA)
			{
				var bytes = Translation_PVA_Values.Create(value.PVA, 1.0f).GetBytes();
				data.Add(bytes.Count().GetBytes());
				data.Add(bytes);
			}
			else if (value.Type.GetValue() == Data.LocationValues.ParamaterType.Easing)
			{
				var easing = Utl.MathUtl.Easing((float)value.Easing.StartSpeed.Value, (float)value.Easing.EndSpeed.Value);

				List<byte[]> _data = new List<byte[]>();
				_data.Add(value.Easing.Start.GetBytes(1.0f));
				_data.Add(value.Easing.End.GetBytes(1.0f));
				_data.Add(BitConverter.GetBytes(easing[0]));
				_data.Add(BitConverter.GetBytes(easing[1]));
				_data.Add(BitConverter.GetBytes(easing[2]));
				var __data = _data.ToArray().ToArray();
				data.Add(__data.Count().GetBytes());
				data.Add(__data);
			}
			else if (value.Type.GetValue() == Data.LocationValues.ParamaterType.LocationFCurve)
			{
				var bytes1 = value.LocationFCurve.FCurve.GetBytes();

				List<byte[]> _data = new List<byte[]>();
				data.Add(bytes1.Count().GetBytes());
				data.Add(bytes1);
			}

			return data.ToArray().ToArray();
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	struct Translation_Fixed_Values
	{
		public Vector3D Position;

		static public Translation_Fixed_Values Create(Data.LocationValues.FixedParamater value, float magnification)
		{
			var s_value = new Translation_Fixed_Values();

			s_value.Position = new Vector3D(
				value.Location.X * magnification,
				value.Location.Y * magnification,
				value.Location.Z * magnification);

			return s_value;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	struct Translation_PVA_Values
	{
		public Vector3D Position_Max;
		public Vector3D Position_Min;

		public Vector3D Velocity_Max;
		public Vector3D Velocity_Min;

		public Vector3D Acceleration_Max;
		public Vector3D Acceleration_Min;

		static public Translation_PVA_Values Create(Data.LocationValues.PVAParamater value, float magnification)
		{
			var s_value = new Translation_PVA_Values();
	
			s_value.Position_Min = new Vector3D(
				value.Location.X.Min * magnification,
				value.Location.Y.Min * magnification,
				value.Location.Z.Min * magnification);

			s_value.Position_Max = new Vector3D(
				value.Location.X.Max * magnification,
				value.Location.Y.Max * magnification,
				value.Location.Z.Max * magnification);

			s_value.Velocity_Min = new Vector3D(
				value.Velocity.X.Min * magnification,
				value.Velocity.Y.Min * magnification,
				value.Velocity.Z.Min * magnification);

			s_value.Velocity_Max = new Vector3D(
				value.Velocity.X.Max * magnification,
				value.Velocity.Y.Max * magnification,
				value.Velocity.Z.Max * magnification);

			s_value.Acceleration_Min = new Vector3D(
				value.Acceleration.X.Min * magnification,
				value.Acceleration.Y.Min * magnification,
				value.Acceleration.Z.Min * magnification);

			s_value.Acceleration_Max = new Vector3D(
				value.Acceleration.X.Max * magnification,
				value.Acceleration.Y.Max * magnification,
				value.Acceleration.Z.Max * magnification);

			return s_value;
		}
	}
}


namespace EffekseerTool.Binary
{
	class RendererCommonValues
	{
		public static byte[] GetBytes(Data.RendererCommonValues value, Dictionary<string, int> texture_and_index, Dictionary<string, int> distortionTexture_and_index)
		{
			List<byte[]> data = new List<byte[]>();

			var texInfo = new TextureInformation();
			var hasTexture = true;

			// テクスチャ番号
			if (value.Distortion.Value)
			{
				if (value.ColorTexture.RelativePath != string.Empty &&
				distortionTexture_and_index.ContainsKey(value.ColorTexture.RelativePath))
				{
					if (texInfo.Load(value.ColorTexture.AbsolutePath))
					{
						data.Add(distortionTexture_and_index[value.ColorTexture.RelativePath].GetBytes());
						hasTexture = true;
					}
					else
					{
						Utils.LogFileNotFound(value.ColorTexture.AbsolutePath);
						data.Add((-1).GetBytes());
						hasTexture = false;
					}
				}
				else
				{
					data.Add((-1).GetBytes());
					hasTexture = false;
				}
			}
			else
			{
				if (value.ColorTexture.RelativePath != string.Empty &&
					texture_and_index.ContainsKey(value.ColorTexture.RelativePath))
				{
					if (texInfo.Load(value.ColorTexture.AbsolutePath))
					{
						data.Add(texture_and_index[value.ColorTexture.RelativePath].GetBytes());
						hasTexture = true;
					}
					else
					{
						Utils.LogFileNotFound(value.ColorTexture.AbsolutePath);
						data.Add((-1).GetBytes());
						hasTexture = false;
					}
				}
				else
				{
					data.Add((-1).GetBytes());
					hasTexture = false;
				}
			}


			data.Add(value.AlphaBlend);
			data.Add(value.Filter);
			data.Add(value.Wrap);

			if (value.ZTest.GetValue())
			{
				data.Add((1).GetBytes());
			}
			else
			{
				data.Add((0).GetBytes());
			}

			if (value.ZWrite.GetValue())
			{
				data.Add((1).GetBytes());
			}
			else
			{
				data.Add((0).GetBytes());
			}

			data.Add(value.FadeInType);
			if (value.FadeInType.Value == Data.RendererCommonValues.FadeType.Use)
			{
				data.Add(value.FadeIn.Frame.GetBytes());

				var easing = Utl.MathUtl.Easing((float)value.FadeIn.StartSpeed.Value, (float)value.FadeIn.EndSpeed.Value);
				data.Add(BitConverter.GetBytes(easing[0]));
				data.Add(BitConverter.GetBytes(easing[1]));
				data.Add(BitConverter.GetBytes(easing[2]));
			}

			data.Add(value.FadeOutType);
			if (value.FadeOutType.Value == Data.RendererCommonValues.FadeType.Use)
			{
				data.Add(value.FadeOut.Frame.GetBytes());

				var easing = Utl.MathUtl.Easing((float)value.FadeOut.StartSpeed.Value, (float)value.FadeOut.EndSpeed.Value);
				data.Add(BitConverter.GetBytes(easing[0]));
				data.Add(BitConverter.GetBytes(easing[1]));
				data.Add(BitConverter.GetBytes(easing[2]));
			}

			if (hasTexture)
			{
				var width = (float)texInfo.Width;
				var height = (float)texInfo.Height;

				data.Add(value.UV);
				if (value.UV.Value == Data.RendererCommonValues.UVType.Default)
				{
				}
				else if (value.UV.Value == Data.RendererCommonValues.UVType.Fixed)
				{
					var value_ = value.UVFixed;
					data.Add((value_.Start.X / width).GetBytes());
					data.Add((value_.Start.Y / height).GetBytes());
					data.Add((value_.Size.X / width).GetBytes());
					data.Add((value_.Size.Y / height).GetBytes());
				}
				else if (value.UV.Value == Data.RendererCommonValues.UVType.Animation)
				{
					var value_ = value.UVAnimation;
					data.Add((value_.Start.X / width).GetBytes());
					data.Add((value_.Start.Y / height).GetBytes());
					data.Add((value_.Size.X / width).GetBytes());
					data.Add((value_.Size.Y / height).GetBytes());

					if (value_.FrameLength.Infinite)
					{
						var inf = int.MaxValue / 100;
						data.Add(inf.GetBytes());
					}
					else
					{
						data.Add(value_.FrameLength.Value.Value.GetBytes());
					}
				
					data.Add(value_.FrameCountX.Value.GetBytes());
					data.Add(value_.FrameCountY.Value.GetBytes());
					data.Add(value_.LoopType);

					data.Add(value_.StartSheet.Max.GetBytes());
					data.Add(value_.StartSheet.Min.GetBytes());

				}
				else if (value.UV.Value == Data.RendererCommonValues.UVType.Scroll)
				{
					var value_ = value.UVScroll;
					data.Add((value_.Start.X.Max / width).GetBytes());
					data.Add((value_.Start.Y.Max / height).GetBytes());
					data.Add((value_.Start.X.Min / width).GetBytes());
					data.Add((value_.Start.Y.Min / height).GetBytes());

					data.Add((value_.Size.X.Max / width).GetBytes());
					data.Add((value_.Size.Y.Max / height).GetBytes());
					data.Add((value_.Size.X.Min / width).GetBytes());
					data.Add((value_.Size.Y.Min / height).GetBytes());

					data.Add((value_.Speed.X.Max / width).GetBytes());
					data.Add((value_.Speed.Y.Max / height).GetBytes());
					data.Add((value_.Speed.X.Min / width).GetBytes());
					data.Add((value_.Speed.Y.Min / height).GetBytes());
				}
				else if (value.UV.Value == Data.RendererCommonValues.UVType.FCurve)
				{
					{
						var value_ = value.UVFCurve.Start;
						var bytes1 = value_.GetBytes(1.0f / width);
						List<byte[]> _data = new List<byte[]>();
						data.Add(bytes1);
					}

					{
						var value_ = value.UVFCurve.Size;
						var bytes1 = value_.GetBytes(1.0f / height);
						List<byte[]> _data = new List<byte[]>();
						data.Add(bytes1);
					}
				}
			}
			else
			{
				data.Add(((int)Data.RendererCommonValues.UVType.Default).GetBytes());
			}

			// Inheritance
			data.Add(value.ColorInheritType.GetValueAsInt().GetBytes());

			// 歪み
			if (value.Distortion.GetValue())
			{
				data.Add((1).GetBytes());
			}
			else
			{
				data.Add((0).GetBytes());
			}

			data.Add(value.DistortionIntensity.GetBytes());

			return data.ToArray().ToArray();
		}
	}
}

namespace EffekseerTool.Binary
{
	class RendererValues
	{
		public static byte[] GetBytes(Data.RendererValues value, Dictionary<string, int> texture_and_index, Dictionary<string, int> normalTexture_and_index, Dictionary<string, int> model_and_index)
		{
			List<byte[]> data = new List<byte[]>();

			if (value == null)
			{
				data.Add(((int)(Data.RendererValues.ParamaterType.None)).GetBytes());
			}
			else
			{
				data.Add(value.Type.GetValueAsInt().GetBytes());
			}
			

			if (value == null)
			{ 
			
			}
			else if (value.Type.Value == Data.RendererValues.ParamaterType.None)
			{
				
			}
			else if (value.Type.Value == Data.RendererValues.ParamaterType.Sprite)
			{
				var param = value.Sprite;

				data.Add(param.RenderingOrder);
				//data.Add(sprite_paramater.AlphaBlend);
				data.Add(param.Billboard);

				// 全体色
				data.Add(param.ColorAll);

				if (param.ColorAll.Value == Data.StandardColorType.Fixed)
				{
					var color_all = (byte[])param.ColorAll_Fixed;
					data.Add(color_all);
				}
				else if (param.ColorAll.Value == Data.StandardColorType.Random)
				{
					var color_random = (byte[])param.ColorAll_Random;
					data.Add(color_random);
				}
				else if (param.ColorAll.Value == Data.StandardColorType.Easing)
				{
					var easing = Utl.MathUtl.Easing((float)value.Sprite.ColorAll_Easing.StartSpeed.Value, (float)value.Sprite.ColorAll_Easing.EndSpeed.Value);
					data.Add((byte[])value.Sprite.ColorAll_Easing.Start);
					data.Add((byte[])value.Sprite.ColorAll_Easing.End);
					data.Add(BitConverter.GetBytes(easing[0]));
					data.Add(BitConverter.GetBytes(easing[1]));
					data.Add(BitConverter.GetBytes(easing[2]));
				}
				else if (param.ColorAll.Value == Data.StandardColorType.FCurve)
				{
					var bytes = param.ColorAll_FCurve.FCurve.GetBytes();
					data.Add(bytes);
				}
				
				// 部分色
				data.Add(param.Color);

				if (param.Color.Value == Data.RendererValues.SpriteParamater.ColorType.Default)
				{
					
				}
				else if (param.Color.Value == Data.RendererValues.SpriteParamater.ColorType.Fixed)
				{
					var color_ll = (byte[])param.Color_Fixed_LL;
					var color_lr = (byte[])param.Color_Fixed_LR;
					var color_ul = (byte[])param.Color_Fixed_UL;
					var color_ur = (byte[])param.Color_Fixed_UR;

					data.Add(color_ll);
					data.Add(color_lr);
					data.Add(color_ul);
					data.Add(color_ur);
					
				}

				// 座標
				//data.Add(sprite_paramater.Position);
				data.Add(BitConverter.GetBytes((int)Data.RendererValues.SpriteParamater.PositionType.Fixed));

				if (param.Position.Value == Data.RendererValues.SpriteParamater.PositionType.Default)
				{
					data.Add(BitConverter.GetBytes(-0.5f));
					data.Add(BitConverter.GetBytes(-0.5f));
					data.Add(BitConverter.GetBytes(+0.5f));
					data.Add(BitConverter.GetBytes(-0.5f));
					data.Add(BitConverter.GetBytes(-0.5f));
					data.Add(BitConverter.GetBytes(+0.5f));
					data.Add(BitConverter.GetBytes(+0.5f));
					data.Add(BitConverter.GetBytes(+0.5f));
				}
				else if (param.Position.Value == Data.RendererValues.SpriteParamater.PositionType.Fixed)
				{
					var pos_ll = (byte[])param.Position_Fixed_LL.GetBytes();
					var pos_lr = (byte[])param.Position_Fixed_LR.GetBytes();
					var pos_ul = (byte[])param.Position_Fixed_UL.GetBytes();
					var pos_ur = (byte[])param.Position_Fixed_UR.GetBytes();

					data.Add(pos_ll);
					data.Add(pos_lr);
					data.Add(pos_ul);
					data.Add(pos_ur);
				}

				// テクスチャ番号
				/*
				if (sprite_paramater.ColorTexture.RelativePath != string.Empty)
				{
					data.Add(texture_and_index[sprite_paramater.ColorTexture.RelativePath].GetBytes());
				}
				else
				{
					data.Add((-1).GetBytes());
				}
				*/
			}
			else if (value.Type.Value == Data.RendererValues.ParamaterType.Ribbon)
			{
				var ribbonParamater = value.Ribbon;

				//data.Add(ribbonParamater.AlphaBlend);

				if (ribbonParamater.ViewpointDependent)
				{
					data.Add((1).GetBytes());
				}
				else
				{
					data.Add((0).GetBytes());
				}

				// 全体色
				data.Add(ribbonParamater.ColorAll);

				if (ribbonParamater.ColorAll.Value == Data.RendererValues.RibbonParamater.ColorAllType.Fixed)
				{
					var color_all = (byte[])ribbonParamater.ColorAll_Fixed;
					data.Add(color_all);
				}
				else if (ribbonParamater.ColorAll.Value == Data.RendererValues.RibbonParamater.ColorAllType.Random)
				{
					var color_random = (byte[])ribbonParamater.ColorAll_Random;
					data.Add(color_random);
				}
				else if (ribbonParamater.ColorAll.Value == Data.RendererValues.RibbonParamater.ColorAllType.Easing)
				{
					var easing = Utl.MathUtl.Easing((float)ribbonParamater.ColorAll_Easing.StartSpeed.Value, (float)ribbonParamater.ColorAll_Easing.EndSpeed.Value);
					data.Add((byte[])ribbonParamater.ColorAll_Easing.Start);
					data.Add((byte[])ribbonParamater.ColorAll_Easing.End);
					data.Add(BitConverter.GetBytes(easing[0]));
					data.Add(BitConverter.GetBytes(easing[1]));
					data.Add(BitConverter.GetBytes(easing[2]));
				}

				// 部分色
				data.Add(ribbonParamater.Color);

				if (ribbonParamater.Color.Value == Data.RendererValues.RibbonParamater.ColorType.Default)
				{

				}
				else if (ribbonParamater.Color.Value == Data.RendererValues.RibbonParamater.ColorType.Fixed)
				{
					var color_l = (byte[])ribbonParamater.Color_Fixed_L;
					var color_r = (byte[])ribbonParamater.Color_Fixed_R;

					data.Add(color_l);
					data.Add(color_r);
				}

				// 座標
				//data.Add(ribbonParamater.Position);
				data.Add(BitConverter.GetBytes((int)Data.RendererValues.RibbonParamater.PositionType.Fixed));

				if (ribbonParamater.Position.Value == Data.RendererValues.RibbonParamater.PositionType.Default)
				{
					data.Add(BitConverter.GetBytes(-0.5f));
					data.Add(BitConverter.GetBytes(+0.5f));
				}
				else if (ribbonParamater.Position.Value == Data.RendererValues.RibbonParamater.PositionType.Fixed)
				{
					var pos_l = (byte[])ribbonParamater.Position_Fixed_L.GetBytes();
					var pos_r = (byte[])ribbonParamater.Position_Fixed_R.GetBytes();
					data.Add(pos_l);
					data.Add(pos_r);
				}

				data.Add(BitConverter.GetBytes(ribbonParamater.SplineDivision.Value));

				// テクスチャ番号
				/*
				if (ribbonParamater.ColorTexture.RelativePath != string.Empty)
				{
					data.Add(texture_and_index[ribbonParamater.ColorTexture.RelativePath].GetBytes());
				}
				else
				{
					data.Add((-1).GetBytes());
				}
				*/
			}
            else if (value.Type.Value == Data.RendererValues.ParamaterType.Ring)
            {
                var ringParamater = value.Ring;

                data.Add(ringParamater.RenderingOrder);
                //data.Add(ringParamater.AlphaBlend);
                data.Add(ringParamater.Billboard);

                data.Add(ringParamater.VertexCount.Value.GetBytes());

                data.Add(ringParamater.ViewingAngle);
                if (ringParamater.ViewingAngle.GetValue() == Data.RendererValues.RingParamater.ViewingAngleType.Fixed)
                {
                    data.Add(ringParamater.ViewingAngle_Fixed.Value.GetBytes());
                }
                else if (ringParamater.ViewingAngle.GetValue() == Data.RendererValues.RingParamater.ViewingAngleType.Random)
                {
                    data.Add(ringParamater.ViewingAngle_Random.Max.GetBytes());
                    data.Add(ringParamater.ViewingAngle_Random.Min.GetBytes());
                }
                else if (ringParamater.ViewingAngle.GetValue() == Data.RendererValues.RingParamater.ViewingAngleType.Easing)
                {
                    var easing = Utl.MathUtl.Easing(
                        (float)ringParamater.ViewingAngle_Easing.StartSpeed.Value,
                        (float)ringParamater.ViewingAngle_Easing.EndSpeed.Value);

                    data.Add(ringParamater.ViewingAngle_Easing.Start.Max.GetBytes());
                    data.Add(ringParamater.ViewingAngle_Easing.Start.Min.GetBytes());
                    data.Add(ringParamater.ViewingAngle_Easing.End.Max.GetBytes());
                    data.Add(ringParamater.ViewingAngle_Easing.End.Min.GetBytes());
                    data.Add(BitConverter.GetBytes(easing[0]));
                    data.Add(BitConverter.GetBytes(easing[1]));
                    data.Add(BitConverter.GetBytes(easing[2]));
                }

                data.Add(ringParamater.Outer);
                if (ringParamater.Outer.GetValue() == Data.RendererValues.RingParamater.LocationType.Fixed)
                {
                    data.Add((ringParamater.Outer_Fixed.Location.X.Value).GetBytes());
					data.Add((ringParamater.Outer_Fixed.Location.Y.Value).GetBytes());
                }
                else if (ringParamater.Outer.GetValue() == Data.RendererValues.RingParamater.LocationType.PVA)
                {
					data.Add(ringParamater.Outer_PVA.Location.GetBytes());
					data.Add(ringParamater.Outer_PVA.Velocity.GetBytes());
					data.Add(ringParamater.Outer_PVA.Acceleration.GetBytes());
                }
                else if (ringParamater.Outer.GetValue() == Data.RendererValues.RingParamater.LocationType.Easing)
                {
                    var easing = Utl.MathUtl.Easing(
                        (float)ringParamater.Outer_Easing.StartSpeed.Value, 
                        (float)ringParamater.Outer_Easing.EndSpeed.Value);

                    data.Add((byte[])ringParamater.Outer_Easing.Start.GetBytes());
                    data.Add((byte[])ringParamater.Outer_Easing.End.GetBytes());
                    data.Add(BitConverter.GetBytes(easing[0]));
                    data.Add(BitConverter.GetBytes(easing[1]));
                    data.Add(BitConverter.GetBytes(easing[2]));
                }

                data.Add(ringParamater.Inner);
                if (ringParamater.Inner.GetValue() == Data.RendererValues.RingParamater.LocationType.Fixed)
                {
					data.Add((ringParamater.Inner_Fixed.Location.X.Value).GetBytes());
					data.Add((ringParamater.Inner_Fixed.Location.Y.Value).GetBytes());
                }
                else if (ringParamater.Inner.GetValue() == Data.RendererValues.RingParamater.LocationType.PVA)
                {
					data.Add(ringParamater.Inner_PVA.Location.GetBytes());
					data.Add(ringParamater.Inner_PVA.Velocity.GetBytes());
					data.Add(ringParamater.Inner_PVA.Acceleration.GetBytes());
                }
                else if (ringParamater.Inner.GetValue() == Data.RendererValues.RingParamater.LocationType.Easing)
                {
                    var easing = Utl.MathUtl.Easing(
                        (float)ringParamater.Inner_Easing.StartSpeed.Value,
                        (float)ringParamater.Inner_Easing.EndSpeed.Value);

                    data.Add((byte[])ringParamater.Inner_Easing.Start.GetBytes());
                    data.Add((byte[])ringParamater.Inner_Easing.End.GetBytes());
                    data.Add(BitConverter.GetBytes(easing[0]));
                    data.Add(BitConverter.GetBytes(easing[1]));
                    data.Add(BitConverter.GetBytes(easing[2]));
                }

                data.Add(ringParamater.CenterRatio);
                if (ringParamater.CenterRatio.GetValue() == Data.RendererValues.RingParamater.CenterRatioType.Fixed)
                {
                    data.Add(ringParamater.CenterRatio_Fixed.Value.GetBytes());
                }
                else if (ringParamater.CenterRatio.GetValue() == Data.RendererValues.RingParamater.CenterRatioType.Random)
                {
                    data.Add(ringParamater.CenterRatio_Random.Max.GetBytes());
                    data.Add(ringParamater.CenterRatio_Random.Min.GetBytes());
                }
                else if (ringParamater.CenterRatio.GetValue() == Data.RendererValues.RingParamater.CenterRatioType.Easing)
                {
                    var easing = Utl.MathUtl.Easing(
                        (float)ringParamater.CenterRatio_Easing.StartSpeed.Value,
                        (float)ringParamater.CenterRatio_Easing.EndSpeed.Value);

                    data.Add(ringParamater.CenterRatio_Easing.Start.Max.GetBytes());
                    data.Add(ringParamater.CenterRatio_Easing.Start.Min.GetBytes());
                    data.Add(ringParamater.CenterRatio_Easing.End.Max.GetBytes());
                    data.Add(ringParamater.CenterRatio_Easing.End.Min.GetBytes());
                    data.Add(BitConverter.GetBytes(easing[0]));
                    data.Add(BitConverter.GetBytes(easing[1]));
                    data.Add(BitConverter.GetBytes(easing[2]));
                }

                data.Add(ringParamater.OuterColor);
                if (ringParamater.OuterColor.Value == Data.RendererValues.RingParamater.ColorType.Fixed)
                {
                    data.Add((byte[])ringParamater.OuterColor_Fixed);
                }
                else if (ringParamater.OuterColor.Value == Data.RendererValues.RingParamater.ColorType.Random)
                {
                    data.Add((byte[])ringParamater.OuterColor_Random);
                }
                else if (ringParamater.OuterColor.Value == Data.RendererValues.RingParamater.ColorType.Easing)
                {
                    var easing = Utl.MathUtl.Easing(
                        (float)ringParamater.OuterColor_Easing.StartSpeed.Value, 
                        (float)ringParamater.OuterColor_Easing.EndSpeed.Value);
                    data.Add((byte[])ringParamater.OuterColor_Easing.Start);
                    data.Add((byte[])ringParamater.OuterColor_Easing.End);
                    data.Add(BitConverter.GetBytes(easing[0]));
                    data.Add(BitConverter.GetBytes(easing[1]));
                    data.Add(BitConverter.GetBytes(easing[2]));
                }

                data.Add(ringParamater.CenterColor);
                if (ringParamater.CenterColor.Value == Data.RendererValues.RingParamater.ColorType.Fixed)
                {
                    data.Add((byte[])ringParamater.CenterColor_Fixed);
                }
                else if (ringParamater.CenterColor.Value == Data.RendererValues.RingParamater.ColorType.Random)
                {
                    data.Add((byte[])ringParamater.CenterColor_Random);
                }
                else if (ringParamater.CenterColor.Value == Data.RendererValues.RingParamater.ColorType.Easing)
                {
                    var easing = Utl.MathUtl.Easing(
                        (float)ringParamater.CenterColor_Easing.StartSpeed.Value,
                        (float)ringParamater.CenterColor_Easing.EndSpeed.Value);
                    data.Add((byte[])ringParamater.CenterColor_Easing.Start);
                    data.Add((byte[])ringParamater.CenterColor_Easing.End);
                    data.Add(BitConverter.GetBytes(easing[0]));
                    data.Add(BitConverter.GetBytes(easing[1]));
                    data.Add(BitConverter.GetBytes(easing[2]));
                }

                data.Add(ringParamater.InnerColor);
                if (ringParamater.InnerColor.Value == Data.RendererValues.RingParamater.ColorType.Fixed)
                {
                    data.Add((byte[])ringParamater.InnerColor_Fixed);
                }
                else if (ringParamater.InnerColor.Value == Data.RendererValues.RingParamater.ColorType.Random)
                {
                    data.Add((byte[])ringParamater.InnerColor_Random);
                }
                else if (ringParamater.InnerColor.Value == Data.RendererValues.RingParamater.ColorType.Easing)
                {
                    var easing = Utl.MathUtl.Easing(
                        (float)ringParamater.InnerColor_Easing.StartSpeed.Value,
                        (float)ringParamater.InnerColor_Easing.EndSpeed.Value);
                    data.Add((byte[])ringParamater.InnerColor_Easing.Start);
                    data.Add((byte[])ringParamater.InnerColor_Easing.End);
                    data.Add(BitConverter.GetBytes(easing[0]));
                    data.Add(BitConverter.GetBytes(easing[1]));
                    data.Add(BitConverter.GetBytes(easing[2]));
                }

                // テクスチャ番号
                /*
				if (ringParamater.ColorTexture.RelativePath != string.Empty)
                {
                    data.Add(texture_and_index[ringParamater.ColorTexture.RelativePath].GetBytes());
                }
                else
                {
                    data.Add((-1).GetBytes());
                }
				*/
            }
			else if (value.Type.Value == Data.RendererValues.ParamaterType.Model)
			{
				var param = value.Model;

				data.Add((1.0f).GetBytes());

				if (param.Model.RelativePath != string.Empty)
				{
					var relative_path = param.Model.RelativePath;

					if (string.IsNullOrEmpty(System.IO.Path.GetDirectoryName(relative_path)))
					{
						relative_path = System.IO.Path.GetFileNameWithoutExtension(relative_path) + ".efkmodel";
					}
					else
					{
						relative_path = System.IO.Path.GetDirectoryName(relative_path) + "/" + System.IO.Path.GetFileNameWithoutExtension(relative_path) + ".efkmodel";
					}

					data.Add(model_and_index[relative_path].GetBytes());
				}
				else
				{
					data.Add((-1).GetBytes());
				}

				if (param.NormalTexture.RelativePath != string.Empty)
				{
					data.Add(normalTexture_and_index[param.NormalTexture.RelativePath].GetBytes());
				}
				else
				{
					data.Add((-1).GetBytes());
				}

				data.Add(param.Billboard);

				if (param.Lighting.Value)
				{
					data.Add((1).GetBytes());
				}
				else
				{
					data.Add((0).GetBytes());
				}

				data.Add(((int)param.Culling.Value).GetBytes());

				// 全体色
				OutputStandardColor(data, param.Color, param.Color_Fixed, param.Color_Random, param.Color_Easing, param.Color_FCurve);
			}
			else if (value.Type.Value == Data.RendererValues.ParamaterType.Track)
			{
				var param = value.Track;
				data.Add(param.TrackSizeFor);
				data.Add(BitConverter.GetBytes(param.TrackSizeFor_Fixed.Value));

				data.Add(param.TrackSizeMiddle);
				data.Add(BitConverter.GetBytes(param.TrackSizeMiddle_Fixed.Value));

				data.Add(param.TrackSizeBack);
				data.Add(BitConverter.GetBytes(param.TrackSizeBack_Fixed.Value));
				
				data.Add(BitConverter.GetBytes(param.SplineDivision.Value));

				OutputStandardColor(data, param.ColorLeft, param.ColorLeft_Fixed, param.ColorLeft_Random, param.ColorLeft_Easing, param.ColorLeft_FCurve);
				OutputStandardColor(data, param.ColorLeftMiddle, param.ColorLeftMiddle_Fixed, param.ColorLeftMiddle_Random, param.ColorLeftMiddle_Easing, param.ColorLeftMiddle_FCurve);

				OutputStandardColor(data, param.ColorCenter, param.ColorCenter_Fixed, param.ColorCenter_Random, param.ColorCenter_Easing, param.ColorCenter_FCurve);
				OutputStandardColor(data, param.ColorCenterMiddle, param.ColorCenterMiddle_Fixed, param.ColorCenterMiddle_Random, param.ColorCenterMiddle_Easing, param.ColorCenterMiddle_FCurve);

				OutputStandardColor(data, param.ColorRight, param.ColorRight_Fixed, param.ColorRight_Random, param.ColorRight_Easing, param.ColorRight_FCurve);
				OutputStandardColor(data, param.ColorRightMiddle, param.ColorRightMiddle_Fixed, param.ColorRightMiddle_Random, param.ColorRightMiddle_Easing, param.ColorRightMiddle_FCurve);
			}

			return data.ToArray().ToArray();
		}

		private static void OutputStandardColor(
			List<byte[]> data, 
			Data.Value.Enum<Data.StandardColorType> color,
			Data.Value.Color color_fixed,
			Data.Value.ColorWithRandom color_Random,
			Data.ColorEasingParamater color_Easing,
			Data.ColorFCurveParameter color_FCurve)
		{
			data.Add(color);

			if (color.Value == Data.StandardColorType.Fixed)
			{
				var color_all = (byte[])color_fixed;
				data.Add(color_all);
			}
			else if (color.Value == Data.StandardColorType.Random)
			{
				var color_random = (byte[])color_Random;
				data.Add(color_random);
			}
			else if (color.Value == Data.StandardColorType.Easing)
			{
				var easing = Utl.MathUtl.Easing((float)color_Easing.StartSpeed.Value, (float)color_Easing.EndSpeed.Value);
				data.Add((byte[])color_Easing.Start);
				data.Add((byte[])color_Easing.End);
				data.Add(BitConverter.GetBytes(easing[0]));
				data.Add(BitConverter.GetBytes(easing[1]));
				data.Add(BitConverter.GetBytes(easing[2]));
			}
			else if (color.Value == Data.StandardColorType.FCurve)
			{
				var bytes = color_FCurve.FCurve.GetBytes();
				data.Add(bytes);
			}
		}
	}
}

namespace EffekseerTool.Binary
{
	class RotationValues
	{
		public static byte[] GetBytes(Data.RotationValues value)
		{
			List<byte[]> data = new List<byte[]>();
			data.Add(value.Type.GetValueAsInt().GetBytes());

			if (value.Type.GetValue() == Data.RotationValues.ParamaterType.Fixed)
			{
				var bytes = Rotation_Fixed_Values.Create(value.Fixed).GetBytes();
				data.Add(bytes.Count().GetBytes());
				data.Add(bytes);
			}
			else if (value.Type.GetValue() == Data.RotationValues.ParamaterType.PVA)
			{
				List<byte[]> _data = new List<byte[]>();
				_data.Add(value.PVA.Rotation.GetBytes((float)Math.PI / 180.0f));
				_data.Add(value.PVA.Velocity.GetBytes((float)Math.PI / 180.0f));
				_data.Add(value.PVA.Acceleration.GetBytes((float)Math.PI / 180.0f));
				var __data = _data.ToArray().ToArray();
				data.Add(__data.Count().GetBytes());
				data.Add(__data);
			}
			else if (value.Type.GetValue() == Data.RotationValues.ParamaterType.Easing)
			{
				var easing = Utl.MathUtl.Easing((float)value.Easing.StartSpeed.Value, (float)value.Easing.EndSpeed.Value);

				List<byte[]> _data = new List<byte[]>();
				_data.Add(value.Easing.Start.GetBytes((float)Math.PI / 180.0f));
				_data.Add(value.Easing.End.GetBytes((float)Math.PI / 180.0f));
				_data.Add(BitConverter.GetBytes(easing[0]));
				_data.Add(BitConverter.GetBytes(easing[1]));
				_data.Add(BitConverter.GetBytes(easing[2]));
				var __data = _data.ToArray().ToArray();
				data.Add(__data.Count().GetBytes());
				data.Add(__data);
			}
			else if (value.Type.GetValue() == Data.RotationValues.ParamaterType.AxisPVA)
			{
				List<byte[]> _data = new List<byte[]>();
				_data.Add(value.AxisPVA.Axis.GetBytes());
				_data.Add(BitConverter.GetBytes(value.AxisPVA.Rotation.Max * (float)Math.PI / 180.0f));
				_data.Add(BitConverter.GetBytes(value.AxisPVA.Rotation.Min * (float)Math.PI / 180.0f));
				_data.Add(BitConverter.GetBytes(value.AxisPVA.Velocity.Max * (float)Math.PI / 180.0f));
				_data.Add(BitConverter.GetBytes(value.AxisPVA.Velocity.Min * (float)Math.PI / 180.0f));
				_data.Add(BitConverter.GetBytes(value.AxisPVA.Acceleration.Max * (float)Math.PI / 180.0f));
				_data.Add(BitConverter.GetBytes(value.AxisPVA.Acceleration.Min * (float)Math.PI / 180.0f));
				var __data = _data.ToArray().ToArray();
				data.Add(__data.Count().GetBytes());
				data.Add(__data);
			}
			else if (value.Type.GetValue() == Data.RotationValues.ParamaterType.AxisEasing)
			{
				var easing = Utl.MathUtl.Easing((float)value.AxisEasing.Easing.StartSpeed.Value, (float)value.AxisEasing.Easing.EndSpeed.Value);

				List<byte[]> _data = new List<byte[]>();
				_data.Add(value.AxisEasing.Axis.GetBytes());
				_data.Add(BitConverter.GetBytes(value.AxisEasing.Easing.Start.Max * (float)Math.PI / 180.0f));
				_data.Add(BitConverter.GetBytes(value.AxisEasing.Easing.Start.Min * (float)Math.PI / 180.0f));
				_data.Add(BitConverter.GetBytes(value.AxisEasing.Easing.End.Max * (float)Math.PI / 180.0f));
				_data.Add(BitConverter.GetBytes(value.AxisEasing.Easing.End.Min * (float)Math.PI / 180.0f));
				_data.Add(BitConverter.GetBytes(easing[0]));
				_data.Add(BitConverter.GetBytes(easing[1]));
				_data.Add(BitConverter.GetBytes(easing[2]));
				var __data = _data.ToArray().ToArray();
				data.Add(__data.Count().GetBytes());
				data.Add(__data);
			}
			else if (value.Type.GetValue() == Data.RotationValues.ParamaterType.RotationFCurve)
			{
				var bytes = value.RotationFCurve.FCurve.GetBytes(
					(float)Math.PI / 180.0f);

				List<byte[]> _data = new List<byte[]>();
				data.Add(bytes.Count().GetBytes());
				data.Add(bytes);
			}

			return data.ToArray().ToArray();
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	struct Rotation_Fixed_Values
	{
		public Vector3D Position;

		static public Rotation_Fixed_Values Create(Data.RotationValues.FixedParamater value)
		{
			var s_value = new Rotation_Fixed_Values();

			s_value.Position = new Vector3D(
				value.Rotation.X * (float)Math.PI / 180.0f,
				value.Rotation.Y * (float)Math.PI / 180.0f,
				value.Rotation.Z * (float)Math.PI / 180.0f);

			return s_value;
		}
	}
}

namespace EffekseerTool.Binary
{
	class ScaleValues
	{
		public static byte[] GetBytes(Data.ScaleValues value, Data.ParentEffectType parentEffectType)
		{
			float magnification = 1.0f;

			List<byte[]> data = new List<byte[]>();
			data.Add(value.Type.GetValueAsInt().GetBytes());

			if (value.Type.GetValue() == Data.ScaleValues.ParamaterType.Fixed)
			{
				var bytes = Scaling_Fixed_Values.Create(value.Fixed,magnification).GetBytes();
				data.Add(bytes.Count().GetBytes());
				data.Add(bytes);
			}
			else if (value.Type.GetValue() == Data.ScaleValues.ParamaterType.PVA)
			{
				var bytes = Scaling_PVA_Values.Create(value.PVA, magnification).GetBytes();
				data.Add(bytes.Count().GetBytes());
				data.Add(bytes);
			}
			else if (value.Type.GetValue() == Data.ScaleValues.ParamaterType.Easing)
			{
				var easing = Utl.MathUtl.Easing(
					(float)value.Easing.StartSpeed.Value,
					(float)value.Easing.EndSpeed.Value);

				List<byte[]> _data = new List<byte[]>();
				_data.Add(value.Easing.Start.GetBytes(magnification));
				_data.Add(value.Easing.End.GetBytes(magnification));
				_data.Add(BitConverter.GetBytes(easing[0]));
				_data.Add(BitConverter.GetBytes(easing[1]));
				_data.Add(BitConverter.GetBytes(easing[2]));
				var __data = _data.ToArray().ToArray();
				data.Add(__data.Count().GetBytes());
				data.Add(__data);
			}
			else if (value.Type.GetValue() == Data.ScaleValues.ParamaterType.SinglePVA)
			{
				var bytes = Scaling_SinglePVA_Values.Create(value.SinglePVA, magnification).GetBytes();
				data.Add(bytes.Count().GetBytes());
				data.Add(bytes);
			}
			else if (value.Type.GetValue() == Data.ScaleValues.ParamaterType.SingleEasing)
			{
				var easing = Utl.MathUtl.Easing(
					(float)value.SingleEasing.StartSpeed.Value,
					(float)value.SingleEasing.EndSpeed.Value);

				List<byte[]> _data = new List<byte[]>();
                _data.Add(value.SingleEasing.Start.Max.GetBytes());
                _data.Add(value.SingleEasing.Start.Min.GetBytes());
                _data.Add(value.SingleEasing.End.Max.GetBytes());
                _data.Add(value.SingleEasing.End.Min.GetBytes());
                _data.Add(BitConverter.GetBytes(easing[0]));
                _data.Add(BitConverter.GetBytes(easing[1]));
                _data.Add(BitConverter.GetBytes(easing[2]));
				var __data = _data.ToArray().ToArray();
				data.Add(__data.Count().GetBytes());
				data.Add(__data);
			}
			else if (value.Type.GetValue() == Data.ScaleValues.ParamaterType.FCurve)
			{
				var bytes = value.FCurve.FCurve.GetBytes();

				data.Add(bytes.Count().GetBytes());
				data.Add(bytes);
			}

			return data.ToArray().ToArray();
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	struct Scaling_Fixed_Values
	{
		public Vector3D Position;

		static public Scaling_Fixed_Values Create(Data.ScaleValues.FixedParamater value, float magnification)
		{
			var s_value = new Scaling_Fixed_Values();

			s_value.Position = new Vector3D(
				value.Scale.X * magnification,
				value.Scale.Y * magnification,
				value.Scale.Z * magnification);

			return s_value;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	struct Scaling_PVA_Values
	{
		public Vector3D Position_Max;
		public Vector3D Position_Min;

		public Vector3D Velocity_Max;
		public Vector3D Velocity_Min;

		public Vector3D Acceleration_Max;
		public Vector3D Acceleration_Min;

		static public Scaling_PVA_Values Create(Data.ScaleValues.PVAParamater value, float magnification)
		{
			var s_value = new Scaling_PVA_Values();

			s_value.Position_Min = new Vector3D(
				value.Scale.X.Min * magnification,
				value.Scale.Y.Min * magnification,
				value.Scale.Z.Min * magnification);

			s_value.Position_Max = new Vector3D(
				value.Scale.X.Max * magnification,
				value.Scale.Y.Max * magnification,
				value.Scale.Z.Max * magnification);

			s_value.Velocity_Min = new Vector3D(
				value.Velocity.X.Min * magnification,
				value.Velocity.Y.Min * magnification,
				value.Velocity.Z.Min * magnification);

			s_value.Velocity_Max = new Vector3D(
				value.Velocity.X.Max * magnification,
				value.Velocity.Y.Max * magnification,
				value.Velocity.Z.Max * magnification);

			s_value.Acceleration_Min = new Vector3D(
				value.Acceleration.X.Min * magnification,
				value.Acceleration.Y.Min * magnification,
				value.Acceleration.Z.Min * magnification);

			s_value.Acceleration_Max = new Vector3D(
				value.Acceleration.X.Max * magnification,
				value.Acceleration.Y.Max * magnification,
				value.Acceleration.Z.Max * magnification);

			return s_value;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	struct Scaling_SinglePVA_Values
	{
		public float Position_Max;
		public float Position_Min;

		public float Velocity_Max;
		public float Velocity_Min;

		public float Acceleration_Max;
		public float Acceleration_Min;

		static public Scaling_SinglePVA_Values Create(Data.ScaleValues.SinglePVAParamater value, float magnification)
		{
			var s_value = new Scaling_SinglePVA_Values();

			s_value.Position_Min = value.Scale.Min * magnification;
			s_value.Position_Max = value.Scale.Max * magnification;
			s_value.Velocity_Min = value.Velocity.Min * magnification;
			s_value.Velocity_Max = value.Velocity.Max * magnification;
			s_value.Acceleration_Min = value.Acceleration.Min * magnification;
			s_value.Acceleration_Max = value.Acceleration.Max * magnification;

			return s_value;
		}
	}
}

namespace EffekseerTool.Binary
{
	class SoundValues
	{
		public static byte[] GetBytes(Data.SoundValues value, Dictionary<string, int> wave_and_index)
		{
			List<byte[]> data = new List<byte[]>();

			if (value == null)
			{
				data.Add(((int)(Data.SoundValues.ParamaterType.None)).GetBytes());
			}
			else
			{
				data.Add(value.Type.GetValueAsInt().GetBytes());
			}
			

			if (value == null)
			{ 
			
			}
            else if (value.Type.Value == Data.SoundValues.ParamaterType.None)
			{
				
			}
			else if (value.Type.Value == Data.SoundValues.ParamaterType.Use)
			{
				var sound_parameter = value.Sound;

                // ウェーブ番号
                if (sound_parameter.Wave.RelativePath != string.Empty)
                {
                    data.Add(wave_and_index[sound_parameter.Wave.RelativePath].GetBytes());
                }
                else
                {
                    data.Add((-1).GetBytes());
                }

                data.Add(sound_parameter.Volume.Max.GetBytes());
                data.Add(sound_parameter.Volume.Min.GetBytes());
                data.Add(sound_parameter.Pitch.Max.GetBytes());
                data.Add(sound_parameter.Pitch.Min.GetBytes());
                data.Add(sound_parameter.PanType.GetValueAsInt().GetBytes());
                data.Add(sound_parameter.Pan.Max.GetBytes());
                data.Add(sound_parameter.Pan.Min.GetBytes());
                data.Add(sound_parameter.Distance.GetValue().GetBytes());
                data.Add(sound_parameter.Delay.Max.GetBytes());
                data.Add(sound_parameter.Delay.Min.GetBytes());
			}

			return data.ToArray().ToArray();
		}
	}
}

namespace EffekseerTool.Command
{
	/// <summary>
	/// 命令管理クラス
	/// </summary>
	public class CommandManager
	{
		public static event EventHandler Changed = null;

		/// <summary>
		/// 結合コマンドを変換してひとつのコマンドに纏める関数群
		/// </summary>
		static List<Func<IEnumerable<ICommand>, ICommand>> convertFunctions = new List<Func<IEnumerable<ICommand>, ICommand>>();

		/// <summary>
		/// 命令履歴
		/// </summary>
		static List<ICommand> cmds = new List<ICommand>();

		/// <summary>
		/// 次の命令を挿入するインデックス
		/// </summary>
		static int cmds_ind = 0;
		
		/// <summary>
		/// 命令をグループ化する際のコンテナ
		/// </summary>
		static List<ICommand> cmd_collections = new List<ICommand>();

		/// <summary>
		/// 命令をグループ化する際の階層数
		/// </summary>
		/// <remarks>
		/// 0の時はグループ化せず、
		/// 1以上の時、グループ化
		/// 0未満の時、例外
		/// </remarks>
		static int cmd_collections_count = 0;

		/// <summary>
		/// 結合中の命令コンテナ
		/// </summary>
		static List<ICommand> cmd_combined = new List<ICommand>();

		/// <summary>
		/// 結合モードか?
		/// </summary>
		static bool isCombinedMode = false;

		/// <summary>
		/// 結合コマンドを変換するメソッドを追加する。
		/// </summary>
		/// <param name="f">コマンドを受け取り、結合できたのなら結合したコマンド、そうでないならnullを返すメソッド</param>
		internal static void AddConvertFunction(Func<IEnumerable<ICommand>, ICommand> f)
		{
			convertFunctions.Add(f);
		}

		/// <summary>
		/// 結合可能なコマンドを結合し、通常のコマンドキューに追加する。
		/// </summary>
		public static void FreezeCombinedCommands()
		{
			if (isCombinedMode)
			{
				if (cmd_combined.Count == 0) throw new InvalidOperationException("List doesn't have combined commands.");

				isCombinedMode = false;

				ICommand cmdCollection = null;

				// 特殊な変換
				foreach (var f in convertFunctions)
				{
					var result = f(cmd_combined);
					if (result != null)
					{
						cmdCollection = result;
						break;
					}
				}

				// 一般的な変換
				if (cmdCollection == null)
				{
					var commandCollection = new CommandCollection();
					foreach (var c in cmd_combined)
					{
						commandCollection.Add(c);
					}
					cmdCollection = commandCollection;
				}

				// 追加処理
				cmd_combined.Clear();

				cmds.Add(cmdCollection);
				cmds_ind++;	
			}
			else
			{
				if (cmd_combined.Count > 0) throw new InvalidOperationException("List has combined commands.");
			}
		}

		/// <summary>
		/// コマンドの実行
		/// </summary>
		/// <param name="command">実行する命令</param>
		internal static void Execute(ICommand command)
		{
			Action<ICommand> addCommand = (c) =>
				{
					var cmd = c as DelegateCommand;
					if (cmd != null && cmd.IsCombined)
					{
						if (cmd_combined.Count > 0) throw new InvalidOperationException("List has combined commands.");

						isCombinedMode = true;
						cmd_combined.Add(c);
					}
					else
					{
						cmds.Add(c);
						cmds_ind++;
					}
				};

			if (command == null) throw new ArgumentNullException("command is null.");

			command.Execute();

			if (cmd_collections_count > 0)
			{
				cmd_collections.Add(command);
			}
			else
			{
				if (cmds.Count > cmds_ind)
				{
					cmds.RemoveRange(cmds_ind, cmds.Count - cmds_ind);
				}

				if (isCombinedMode)
				{
					if (cmd_combined.Count == 0) throw new InvalidOperationException("List doesn't have combined commands.");

					var cmd = command as DelegateCommand;
					var firstCmd = cmd_combined.First() as DelegateCommand;

					if (cmd != null && cmd.IsCombined && cmd.Identifier == firstCmd.Identifier)
					{
						cmd_combined.Add(command);
					}
					else
					{
						FreezeCombinedCommands();

						addCommand(command);
					}
				}
				else
				{
					addCommand(command);
				}
			}

			if (Changed != null)
			{
				Changed(null, null);
			}
		}

		/// <summary>
		/// ロールバック
		/// </summary>
		/// <returns></returns>
		public static bool Undo()
		{
			if (cmd_collections_count > 0)
			{
				return false;
			}
			else
			{
				FreezeCombinedCommands();

				if (cmds_ind > 0)
				{
					cmds_ind--;
					cmds[cmds_ind].Unexecute();

					if (Changed != null)
					{
						Changed(null, null);
					}

					return true;
				}
			}
			
			return false;
		}

		/// <summary>
		/// 再ロールバック
		/// </summary>
		/// <returns></returns>
		public static bool Redo()
		{
			if (cmd_collections_count > 0)
			{
				return false;
			}
			else if( cmds_ind < cmds.Count)
			{
				if (isCombinedMode) throw new InvalidOperationException("CombinedMode is true.");

				cmds[cmds_ind].Execute();
				
				cmds_ind++;

				if (Changed != null)
				{
					Changed(null, null);
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// コマンドのグループ化開始
		/// </summary>
		public static void StartCollection()
		{
			FreezeCombinedCommands();
			cmd_collections_count++;
		}

		/// <summary>
		/// コマンドのグループ化終了
		/// </summary>
		public static void EndCollection()
		{
			if (cmd_collections_count == 0) throw new InvalidOperationException("Collection has not yet start.");
			cmd_collections_count--;

			if (cmd_collections_count == 0)
			{
				if (cmd_collections.Count == 0) return;

				var command = new CommandCollection();
				foreach (var cmd in cmd_collections)
				{
					command.Add(cmd);
				}

				if (cmds.Count > cmds_ind)
				{
					cmds.RemoveRange(cmds_ind, cmds.Count - cmds_ind);
				}

				cmds.Add(command);
				cmds_ind++;

				cmd_collections.Clear();
			}
		}

		/// <summary>
		/// 実行履歴の消去
		/// </summary>
		internal static bool Clear()
		{
			if (cmd_collections_count > 0)
			{
				return false;
			}
			else
			{
				cmds.Clear();
				cmds_ind = 0;
				return true;
			}
		}

		/// <summary>
		/// コマンド群コンテナ
		/// </summary>
		class CommandCollection : ICommand
		{
			List<ICommand> cmds = new List<ICommand>();

			public CommandCollection()
			{ 
			
			}

			public void Add(ICommand cmd)
			{
				cmds.Add(cmd);
			}

			public void Execute()
			{
				foreach (var cmd in cmds)
				{
					cmd.Execute();
				}
			}

			public void Unexecute()
			{
				for (int i = cmds.Count - 1; i >= 0; i--)
				{
					cmds[i].Unexecute();
				}
			}
		}
	}
}

namespace EffekseerTool.Command
{
	/// <summary>
	/// Delegateで実行内容が指定可能な命令
	/// </summary>
	internal class DelegateCommand : ICommand
	{
		System.Action _execute = null;

		System.Action _unexecute = null;

		public object Identifier
		{
			get;
			private set;
		}

		public bool IsCombined
		{
			get;
			private set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="execute">通常実行及び、Redoの際に実行されるメソッド</param>
		/// <param name="unexecute">Undoの際に実行されるメソッド</param>
		/// <param name="identifier">命令を結合する際の識別子</param>
		/// <param name="isCombined">命令を結合するか、識別子が等しいコマンドが続く時、Undo、Redoにて一括で実行される。</param>
		public DelegateCommand(System.Action execute, System.Action unexecute, object identifier = null, bool isCombined = false)
		{
			_execute = execute;
			_unexecute = unexecute;
			Identifier = identifier;
			IsCombined = isCombined;
		}

		public void Execute()
		{
			_execute();
		}

		public void Unexecute()
		{
			_unexecute();
		}
	}
}

namespace EffekseerTool.Command
{
	/// <summary>
	/// 命令インターフェース
	/// </summary>
	internal interface ICommand
	{
		/// <summary>
		/// コマンドの実行
		/// </summary>
		void Execute();

		/// <summary>
		/// コマンドのロールバック
		/// </summary>
		void Unexecute();
	}
}

namespace EffekseerTool.Data
{
	public class CommonValues
	{
		[Name(language = Language.Japanese, value = "生成数")]
		[Description(language = Language.Japanese, value = "インスタンスの生成数")]
		[Name(language = Language.English, value = "Spawn Count")]
		[Description(language = Language.English, value = "Number of instances to generate")]
		public Value.IntWithInifinite MaxGeneration
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "位置への影響")]
		[Description(language = Language.Japanese, value = "親ノードからの位置への影響")]
		[Name(language = Language.English, value = "Inherit Position")]
		[Description(language = Language.English, value = "When this instance should copy its parent node's position")]
		public Value.Enum<ParentEffectType> LocationEffectType
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "回転への影響")]
		[Description(language = Language.Japanese, value = "親ノードからの回転への影響")]
		[Name(language = Language.English, value = "Inherit Rotation")]
		[Description(language = Language.English, value = "When this instance should copy its parent node's rotation")]
		public Value.Enum<ParentEffectType> RotationEffectType
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "拡大への影響")]
		[Description(language = Language.Japanese, value = "親ノードからの拡大への影響")]
		[Name(language = Language.English, value = "Inherit Scale")]
		[Description(language = Language.English, value = "When this instance should copy its parent node's scale")]
		public Value.Enum<ParentEffectType> ScaleEffectType
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "寿命により削除")]
		[Name(language = Language.English, value = "Destroy after time")]
		public Value.Boolean RemoveWhenLifeIsExtinct
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "親削除時削除")]
		[Name(language = Language.English, value = "Destroy with parent")]
		public Value.Boolean RemoveWhenParentIsRemoved
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "子が全て消滅時削除")]
		[Name(language = Language.English, value = "Destroy when no\nmore children")]
		public Value.Boolean RemoveWhenAllChildrenAreRemoved
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "生存時間")]
		[Description(language = Language.Japanese, value = "1インスタンスが生存する時間")]
		[Name(language = Language.English, value = "Time to live")]
		[Description(language = Language.English, value = "Length of time each instance survives")]
		public Value.IntWithRandom Life
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "生成時間")]
		[Description(language = Language.Japanese, value = "1インスタンスを生成するのに必要とする時間")]
		[Name(language = Language.English, value = "Spawn Rate")]
		[Description(language = Language.English, value = "Time between each instance generation")]
		public Value.FloatWithRandom GenerationTime
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "生成開始時間")]
		[Description(language = Language.Japanese, value = "このノードのインスタンスが生成されてから生成を開始するまでの時間")]
		[Name(language = Language.English, value = "Initial Delay")]
		[Description(language = Language.English, value = "Amount of time that must elapse after instance spawns before it starts generating.")]
		public Value.FloatWithRandom GenerationTimeOffset
		{
			get;
			private set;
		}

		internal CommonValues()
		{
			MaxGeneration = new Value.IntWithInifinite(1, false, int.MaxValue, 1);
			LocationEffectType = new Value.Enum<ParentEffectType>(ParentEffectType.Already);
			RotationEffectType = new Value.Enum<ParentEffectType>(ParentEffectType.Already);
			ScaleEffectType = new Value.Enum<ParentEffectType>(ParentEffectType.Already);
			RemoveWhenLifeIsExtinct = new Value.Boolean(true);
			RemoveWhenParentIsRemoved = new Value.Boolean(false);
			RemoveWhenAllChildrenAreRemoved = new Value.Boolean(false);
			Life = new Value.IntWithRandom(100, int.MaxValue, 1);
			GenerationTime = new Value.FloatWithRandom(1.0f, float.MaxValue, 0.00001f);
			GenerationTimeOffset = new Value.FloatWithRandom(0, float.MaxValue, float.MinValue);
		}
	}
}

namespace EffekseerTool.Data
{
    public enum RecordingExporterType : int
    {
        Sprite,
        SpriteSheet,
        Gif,
        Avi,
    }

    public enum RecordingTransparentMethodType : int
    {
        None,
        Original,
        GenerateAlpha,
    }

    public enum ParentEffectType : int
	{
		[Name(value = "なし", language = Language.Japanese)]
		[Name(value = "Never", language = Language.English)]
		NotBind = 0,
		[Name(value = "なし(Root依存)-非推奨", language = Language.Japanese)]
		[Name(value = "Root dependent (deprecated)", language = Language.English)]
		NotBind_Root = 3,
		[Name(value = "生成時のみ", language = Language.Japanese)]
		[Name(value = "Only on create", language = Language.English)]
		WhenCreating = 1,
		[Name(value = "常時", language = Language.Japanese)]
		[Name(value = "Always", language = Language.English)]
		Already = 2,
	}

	public enum AlphaBlendType : int
	{
		[Name(value = "不透明", language = Language.Japanese)]
		[Name(value = "Opacity", language = Language.English)]
		Opacity = 0,
		[Name(value = "通常", language = Language.Japanese)]
		[Name(value = "Blend", language = Language.English)]
		Blend = 1,
		[Name(value = "加算", language = Language.Japanese)]
		[Name(value = "Additive", language = Language.English)]
		Add = 2,
		[Name(value = "減算", language = Language.Japanese)]
		[Name(value = "Subtract", language = Language.English)]
		Sub = 3,
		[Name(value = "乗算", language = Language.Japanese)]
		[Name(value = "Multiply", language = Language.English)]
		Mul = 4,
	}

	public enum RenderingOrder : int
	{
		[Name(value = "生成順", language = Language.Japanese)]
		[Name(value = "Order of spawn", language = Language.English)]
		FirstCreatedInstanceIsFirst = 0,
		[Name(value = "生成順の逆", language = Language.Japanese)]
		[Name(value = "Reversed", language = Language.English)]
		FirstCreatedInstanceIsLast = 1,
	}

	public enum CullingValues : int
	{
		[Name(value = "表表示", language = Language.Japanese)]
		[Name(value = "Front view", language = Language.English)]
		Front = 0,
		[Name(value = "裏表示", language = Language.Japanese)]
		[Name(value = "Back view", language = Language.English)]
		Back = 1,
		[Name(value = "両面表示", language = Language.Japanese)]
		[Name(value = "Double-sided", language = Language.English)]
		Double = 2,
	}

	public enum EasingStart : int
	{
		[Name(value = "低速3", language = Language.Japanese)]
		[Name(value = "Slowest", language = Language.English)]
		StartSlowly3 = -30,
		[Name(value = "低速2", language = Language.Japanese)]
		[Name(value = "Slower", language = Language.English)]
		StartSlowly2 = -20,
		[Name(value = "低速1", language = Language.Japanese)]
		[Name(value = "Slow", language = Language.English)]
		StartSlowly1 = -10,
		[Name(value = "等速", language = Language.Japanese)]
		[Name(value = "Normal", language = Language.English)]
		Start = 0,
		[Name(value = "高速1", language = Language.Japanese)]
		[Name(value = "Fast", language = Language.English)]
		StartRapidly1 = 10,
		[Name(value = "高速2", language = Language.Japanese)]
		[Name(value = "Faster", language = Language.English)]
		StartRapidly2 = 20,
		[Name(value = "高速3", language = Language.Japanese)]
		[Name(value = "Fastest", language = Language.English)]
		StartRapidly3 = 30,
	}

	public enum EasingEnd : int
	{
		[Name(value = "低速3", language = Language.Japanese)]
		[Name(value = "Slowest", language = Language.English)]
		EndSlowly3 = -30,
		[Name(value = "低速2", language = Language.Japanese)]
		[Name(value = "Slower", language = Language.English)]
		EndSlowly2 = -20,
		[Name(value = "低速1", language = Language.Japanese)]
		[Name(value = "Slow", language = Language.English)]
		EndSlowly1 = -10,
		[Name(value = "等速", language = Language.Japanese)]
		[Name(value = "Normal", language = Language.English)]
		End = 0,
		[Name(value = "高速1", language = Language.Japanese)]
		[Name(value = "Fast", language = Language.English)]
		EndRapidly1 = 10,
		[Name(value = "高速2", language = Language.Japanese)]
		[Name(value = "Faster", language = Language.English)]
		EndRapidly2 = 20,
		[Name(value = "高速3", language = Language.Japanese)]
		[Name(value = "Fastest", language = Language.English)]
		EndRapidly3 = 30,
	}

	public enum DrawnAs : int
	{ 
		MaxAndMin,
		CenterAndAmplitude,
	}
	
	public enum ColorSpace : int
	{ 
		RGBA,
		HSVA,
	}

	public enum StandardColorType : int
	{
		[Name(value = "固定", language = Language.Japanese)]
		[Name(value = "Fixed", language = Language.English)]
		Fixed = 0,
		[Name(value = "ランダム", language = Language.Japanese)]
		[Name(value = "Random", language = Language.English)]
		Random = 1,
		[Name(value = "イージング", language = Language.Japanese)]
		[Name(value = "Easing", language = Language.English)]
		Easing = 2,
		[Name(value = "Fカーブ(RGBA)", language = Language.Japanese)]
		[Name(value = "F-Curve (RGBA)", language = Language.English)]
		FCurve = 3,
	}

	public enum TrackSizeType : int
	{
		[Name(value = "固定", language = Language.Japanese)]
		[Name(value = "Fixed", language = Language.English)]
		Fixed = 0,
	}

	public class ColorEasingParamater
	{ 
		[Name(language = Language.Japanese, value = "始点")]
		[Description(language = Language.Japanese, value = "イージングの始点")]
		[Name(language = Language.English, value = "Start")]
		[Description(language = Language.English, value = "Starting point of easing")]
		public Value.ColorWithRandom Start
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "終点")]
		[Description(language = Language.Japanese, value = "イージングの終点")]
		[Name(language = Language.English, value = "End")]
		[Description(language = Language.English, value = "Value of easing at end")]
		public Value.ColorWithRandom End
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "始点速度")]
		[Description(language = Language.Japanese, value = "始点速度")]
		[Name(language = Language.English, value = "Ease In")]
		[Description(language = Language.English, value = "Initial rate of easing")]
		public Value.Enum<EasingStart> StartSpeed
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "終点速度")]
		[Description(language = Language.Japanese, value = "終点速度")]
		[Name(language = Language.English, value = "Ease Out")]
		[Description(language = Language.English, value = "Rate of easing at end")]
		public Value.Enum<EasingEnd> EndSpeed
		{
			get;
			private set;
		}

		internal ColorEasingParamater()
		{
			Start = new Value.ColorWithRandom(255, 255, 255, 255);
			End = new Value.ColorWithRandom(255, 255, 255, 255);
			Start.Link = End;
			End.Link = Start;
			StartSpeed = new Value.Enum<EasingStart>(EasingStart.Start);
			EndSpeed = new Value.Enum<EasingEnd>(EasingEnd.End);
		}
	}

	public class ColorFCurveParameter
	{
		[Name(language = Language.Japanese, value = "Fカーブ")]
		[Description(language = Language.Japanese, value = "Fカーブ")]
		[Name(language = Language.English, value = "F-Curve")]
		[Description(language = Language.English, value = "F-Curve")]
		[Shown(Shown = true)]
		public Value.FCurveColorRGBA FCurve
		{
			get;
			private set;
		}

		internal ColorFCurveParameter()
		{
			FCurve = new Value.FCurveColorRGBA();

			FCurve.R.DefaultValueRangeMax = 256.0f;
			FCurve.R.DefaultValueRangeMin = 0.0f;
			FCurve.G.DefaultValueRangeMax = 256.0f;
			FCurve.G.DefaultValueRangeMin = 0.0f;
			FCurve.B.DefaultValueRangeMax = 256.0f;
			FCurve.B.DefaultValueRangeMin = 0.0f;
			FCurve.A.DefaultValueRangeMax = 256.0f;
			FCurve.A.DefaultValueRangeMin = 0.0f;
		}
	}

    public class FloatEasingParamater
    {
        [Name(language = Language.Japanese, value = "始点")]
        [Description(language = Language.Japanese, value = "イージングの始点")]
		[Name(language = Language.English, value = "Start")]
		[Description(language = Language.English, value = "Starting point of easing")]
        public Value.FloatWithRandom Start
        {
            get;
            private set;
        }

        [Name(language = Language.Japanese, value = "終点")]
        [Description(language = Language.Japanese, value = "イージングの終点")]
		[Name(language = Language.English, value = "End")]
		[Description(language = Language.English, value = "Value of easing at end")]
        public Value.FloatWithRandom End
        {
            get;
            private set;
        }

        [Name(language = Language.Japanese, value = "始点速度")]
        [Description(language = Language.Japanese, value = "始点速度")]
		[Name(language = Language.English, value = "Ease In")]
		[Description(language = Language.English, value = "Initial rate of easing")]
        public Value.Enum<EasingStart> StartSpeed
        {
            get;
            private set;
        }

        [Name(language = Language.Japanese, value = "終点速度")]
        [Description(language = Language.Japanese, value = "終点速度")]
		[Name(language = Language.English, value = "Ease Out")]
		[Description(language = Language.English, value = "Rate of easing at end")]
        public Value.Enum<EasingEnd> EndSpeed
        {
            get;
            private set;
        }

        internal FloatEasingParamater(float value = 0.0f, float max = float.MaxValue, float min = float.MinValue)
        {
            Start = new Value.FloatWithRandom(value, max, min);
            End = new Value.FloatWithRandom(value, max, min);
            StartSpeed = new Value.Enum<EasingStart>(EasingStart.Start);
            EndSpeed = new Value.Enum<EasingEnd>(EasingEnd.End);
        }
    }

    public class Vector2DEasingParamater
    {
        [Name(language = Language.Japanese, value = "始点")]
        [Description(language = Language.Japanese, value = "イージングの始点")]
		[Name(language = Language.English, value = "Start")]
		[Description(language = Language.English, value = "Starting point of easing")]
        public Value.Vector2DWithRandom Start
        {
            get;
            private set;
        }

        [Name(language = Language.Japanese, value = "終点")]
        [Description(language = Language.Japanese, value = "イージングの終点")]
		[Name(language = Language.English, value = "End")]
		[Description(language = Language.English, value = "Value of easing at end")]
        public Value.Vector2DWithRandom End
        {
            get;
            private set;
        }

        [Name(language = Language.Japanese, value = "始点速度")]
        [Description(language = Language.Japanese, value = "始点速度")]
		[Name(language = Language.English, value = "Ease In")]
		[Description(language = Language.English, value = "Initial rate of easing")]
        public Value.Enum<EasingStart> StartSpeed
        {
            get;
            private set;
        }

        [Name(language = Language.Japanese, value = "終点速度")]
        [Description(language = Language.Japanese, value = "終点速度")]
		[Name(language = Language.English, value = "Ease Out")]
		[Description(language = Language.English, value = "Rate of easing at end")]
        public Value.Enum<EasingEnd> EndSpeed
        {
            get;
            private set;
        }

        internal Vector2DEasingParamater()
        {
            Start = new Value.Vector2DWithRandom(0, 0);
            End = new Value.Vector2DWithRandom(0, 0);
            StartSpeed = new Value.Enum<EasingStart>(EasingStart.Start);
            EndSpeed = new Value.Enum<EasingEnd>(EasingEnd.End);
        }
    }

	public class Vector3DEasingParamater
	{
		[Name(language = Language.Japanese, value = "始点")]
		[Description(language = Language.Japanese, value = "イージングの始点")]
		[Name(language = Language.English, value = "Start")]
		[Description(language = Language.English, value = "Starting point of easing")]
		public Value.Vector3DWithRandom Start
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "終点")]
		[Description(language = Language.Japanese, value = "イージングの終点")]
		[Name(language = Language.English, value = "End")]
		[Description(language = Language.English, value = "Value of easing at end")]
		public Value.Vector3DWithRandom End
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "始点速度")]
		[Description(language = Language.Japanese, value = "始点速度")]
		[Name(language = Language.English, value = "Ease In")]
		[Description(language = Language.English, value = "Initial rate of easing")]
		public Value.Enum<EasingStart> StartSpeed
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "終点速度")]
		[Description(language = Language.Japanese, value = "終点速度")]
		[Name(language = Language.English, value = "Ease Out")]
		[Description(language = Language.English, value = "Rate of easing at end")]
		public Value.Enum<EasingEnd> EndSpeed
		{
			get;
			private set;
		}

		internal Vector3DEasingParamater(float defaultX = 0.0f, float defaultY = 0.0f, float defaultZ = 0.0f)
		{
			Start = new Value.Vector3DWithRandom(defaultX, defaultY, defaultZ);
			End = new Value.Vector3DWithRandom(defaultX, defaultY, defaultZ);
			StartSpeed = new Value.Enum<EasingStart>(EasingStart.Start);
			EndSpeed = new Value.Enum<EasingEnd>(EasingEnd.End);
		}
	}

	public class Vector3DFCurveParameter
	{
		[Name(language = Language.Japanese, value = "Fカーブ")]
		[Description(language = Language.Japanese, value = "Fカーブ")]
		[Name(language = Language.English, value = "F-Curve")]
		[Description(language = Language.English, value = "F-Curve")]
		[Shown(Shown = true)]
		public Value.FCurveVector3D FCurve
		{
			get;
			private set;
		}

		public Vector3DFCurveParameter()
		{
			FCurve = new Value.FCurveVector3D();
			
			FCurve.X.DefaultValueRangeMax = 10.0f;
			FCurve.X.DefaultValueRangeMin = -10.0f;
			FCurve.Y.DefaultValueRangeMax = 10.0f;
			FCurve.Y.DefaultValueRangeMin = -10.0f;
			FCurve.Z.DefaultValueRangeMax = 10.0f;
			FCurve.Z.DefaultValueRangeMin = -10.0f;
		}
	}

	/// <summary>
	/// 入出力に関する属性
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Property | AttributeTargets.Field,
	AllowMultiple = false,
	Inherited = false)]
	public class IOAttribute : Attribute
	{
		public IOAttribute()
		{
			Import = true;
			Export = true;
		}

		public bool Import
		{
			get;
			set;
		}

		public bool Export
		{
			get;
			set;
		}
	}

	/// <summary>
	/// 表示に関する属性
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Property | AttributeTargets.Field,
	AllowMultiple = false,
	Inherited = false)]
	public class ShownAttribute : Attribute
	{
		public ShownAttribute()
		{
			Shown = true;
		}

		public bool Shown
		{
			get;
			set;
		}
	}

	/// <summary>
	/// Undoに関する属性
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Property | AttributeTargets.Field,
	AllowMultiple = false,
	Inherited = false)]
	public class UndoAttribute : Attribute
	{
		public UndoAttribute()
		{
			Undo = true;
		}

		public bool Undo
		{
			get;
			set;
		}
	}

	/// <summary>
	/// 選択肢に関する属性
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Property | AttributeTargets.Field,
	AllowMultiple = false,
	Inherited = false)]
	public class SelectorAttribute : Attribute
	{
		public int ID
		{
			get;
			set;
		}

		public SelectorAttribute()
		{
			ID = -1;
		}
	}

	[AttributeUsage(
		AttributeTargets.Property | AttributeTargets.Field,
	AllowMultiple = false,
	Inherited = false)]
	public class SelectedAttribute : Attribute
	{
		public int ID
		{
			get;
			set;
		}

		public int Value
		{
			get;
			set;
		}

		public SelectedAttribute()
		{
			ID = -1;
			Value = -1;
		}
	}
}

namespace EffekseerTool.Data
{
	public enum ZSortType
	{
		[Name(value = "なし", language = Language.Japanese)]
		[Name(value = "None", language = Language.English)]
		None,

		[Name(value = "正順", language = Language.Japanese)]
		[Name(value = "Normal order", language = Language.English)]
		NormalOrder,

		[Name(value = "逆順", language = Language.Japanese)]
		[Name(value = "Reverse order", language = Language.English)]
		ReverseOrder,

	}


	public class DepthValues
    {

        [Name(language = Language.Japanese, value = "Zオフセット")]
        [Description(language = Language.Japanese, value = "描画時に奥行方向に移動されるオフセット")]
        [Name(language = Language.English, value = "Z-Offset")]
        [Description(language = Language.English, value = "An offset which shift Z direction when the particle is rendered.")]
        public Value.Float DepthOffset
        {
            get;
            private set;
        }

		[Name(language = Language.Japanese, value = "Zオフセットによる拡大無効化")]
		[Description(language = Language.Japanese, value = "Zオフセットにより大きさが変化しないようにするか")]
		[Name(language = Language.English, value = "Is scaling invalid \nwith Z offset")]
		[Description(language = Language.English, value = "Whether Scaling is not changed with Z offset")]
		public Value.Boolean IsScaleChangedDependingOnDepthOffset
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "Zオフセットの拡大")]
		[Description(language = Language.Japanese, value = "Zオフセットがパーティクルの拡大で大きくなるか")]
		[Name(language = Language.English, value = "Scaling Z-Offset")]
		[Description(language = Language.English, value = "Whether Z offset is enlarged with a scaling of particles")]
		public Value.Boolean IsDepthOffsetChangedDependingOnParticleScale
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "深度ソート")]
		[Description(language = Language.Japanese, value = "距離による並び替え")]
		[Name(language = Language.English, value = "Depth sort")]
		[Description(language = Language.English, value = "Sorting by a distance")]
		public Value.Enum<ZSortType> ZSort
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "描画優先度")]
		[Description(language = Language.Japanese, value = "小さい描画優先度のノードが先に描画される")]
		[Name(language = Language.English, value = "Drawing priority")]
		[Description(language = Language.English, value = "A node with Small drawing priority is drawn previously")]
		public Value.Int DrawingPriority
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "ソフトパーティクル")]
		[Description(language = Language.Japanese, value = "ソフトパーティクル")]
		[Name(language = Language.English, value = "Soft particle")]
		[Description(language = Language.English, value = "Soft particle")]
		public Value.Float SoftParticle
		{
			get;
			private set;
		}

        public DepthValues()
        {
            DepthOffset = new Value.Float();
			IsScaleChangedDependingOnDepthOffset = new Value.Boolean();
			IsDepthOffsetChangedDependingOnParticleScale = new Value.Boolean();
			ZSort = new Value.Enum<ZSortType>(ZSortType.None);
			DrawingPriority = new Value.Int(0, 255, -255);
			SoftParticle = new Value.Float(0, float.MaxValue, 0.0f);
        }
    }
}

namespace EffekseerTool.Data
{
	public class EffectBehaviorValues
	{
		[Name(language = Language.Japanese, value = "初期位置")]
		[Description(language = Language.Japanese, value = "中心の初期位置")]
		[Name(language = Language.English, value = "Pos")]
		[Description(language = Language.English, value = "Based on center")]
		[Undo(Undo = false)]
		public Value.Vector3D Location
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "移動速度")]
		[Description(language = Language.Japanese, value = "中心の移動速度")]
		[Name(language = Language.English, value = "Speed")]
		[Description(language = Language.English, value = "Starting velocity")]
		[Undo(Undo = false)]
		public Value.Vector3D LocationVelocity
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "初期角度")]
		[Description(language = Language.Japanese, value = "中心の初期角度")]
		[Name(language = Language.English, value = "Angle")]
		[Description(language = Language.English, value = "Rotated about center")]
		[Undo(Undo = false)]
		public Value.Vector3D Rotation
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "回転速度")]
		[Description(language = Language.Japanese, value = "中心の回転速度")]
		[Name(language = Language.English, value = "Angular\nSpeed")]
		[Description(language = Language.English, value = "Rotated about center")]
		[Undo(Undo = false)]
		public Value.Vector3D RotationVelocity
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "初期拡大率")]
		[Description(language = Language.Japanese, value = "中心の初期拡大率")]
		[Name(language = Language.English, value = "Scale")]
		[Description(language = Language.English, value = "Scaled about center")]
		[Undo(Undo = false)]
		public Value.Vector3D Scale
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "拡大速度")]
		[Description(language = Language.Japanese, value = "中心の拡大速度")]
		[Name(language = Language.English, value = "Expansion\nRate")]
		[Description(language = Language.English, value = "Scaled about center")]
		[Undo(Undo = false)]
		public Value.Vector3D ScaleVelocity
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "破棄フレーム")]
		[Description(language = Language.Japanese, value = "中心が破棄されるフレーム")]
		[Name(language = Language.English, value = "Life")]
		[Description(language = Language.English, value = "Frame in which instance is destroyed")]
		[Undo(Undo = false)]
		public Value.IntWithInifinite RemovedTime
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "個数(X方向)")]
		[Description(language = Language.Japanese, value = "エフェクトの個数(X方向)")]
		[Name(language = Language.English, value = "X Count")]
		[Description(language = Language.English, value = "Number of instances spawned about the x-axis")]
		[Undo(Undo = false)]
		public Value.Int CountX
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "個数(Y方向)")]
		[Description(language = Language.Japanese, value = "エフェクトの個数(Y方向)")]
		[Name(language = Language.English, value = "Y Count")]
		[Description(language = Language.English, value = "Number of instances spawned about the y-axis")]
		[Undo(Undo = false)]
		public Value.Int CountY
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "個数(Z方向)")]
		[Description(language = Language.Japanese, value = "エフェクトの個数(Z方向)")]
		[Name(language = Language.English, value = "Z Count")]
		[Description(language = Language.English, value = "Number of instances spawned about the z-axis")]
		[Undo(Undo = false)]
		public Value.Int CountZ
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "距離")]
		[Description(language = Language.Japanese, value = "エフェクト間の距離")]
		[Name(language = Language.English, value = "Separation")]
		[Description(language = Language.English, value = "Distance between the spawned instances")]
		[Undo(Undo = false)]
		public Value.Float Distance
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "生成間隔(フレーム)")]
		[Description(language = Language.Japanese, value = "1以上の場合、指定された時間ごとにエフェクトを生成する。")]
		[Name(language = Language.English, value = "Generating time span(Frame)")]
		[Description(language = Language.English, value = "If the value is larger than 1, effects are generated every specified time")]
		[Undo(Undo = false)]
		public Value.Int TimeSpan
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "全体色")]
		[Name(language = Language.English, value = "Color All")]
		[Undo(Undo = false)]
		public Value.Color ColorAll
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "ターゲット位置")]
		[Description(language = Language.Japanese, value = "ターゲット位置または引力位置")]
		[Name(language = Language.English, value = "Point of\nAttraction")]
		[Description(language = Language.English, value = "Point in space the instances are drawn to")]
		[Undo(Undo = false)]
		public Value.Vector3D TargetLocation
		{
			get;
			private set;
		}

		public EffectBehaviorValues()
		{
			Location = new Value.Vector3D();
			Rotation = new Value.Vector3D();
			Scale = new Value.Vector3D(1.0f, 1.0f, 1.0f);

			LocationVelocity = new Value.Vector3D();
			RotationVelocity = new Value.Vector3D();
			ScaleVelocity = new Value.Vector3D();

			RemovedTime = new Value.IntWithInifinite(0, true, int.MaxValue, 0);

			CountX = new Value.Int(1, int.MaxValue, 1);
			CountY = new Value.Int(1, int.MaxValue, 1);
			CountZ = new Value.Int(1, int.MaxValue, 1);

			Distance = new Value.Float(5.0f, float.MaxValue, 0.0f);

			TimeSpan = new Value.Int(0);
			ColorAll = new Value.Color(255, 255, 255, 255);

			TargetLocation = new Value.Vector3D();
		}

		/// <summary>
		/// Adhoc code
		/// </summary>
		public void Reset()
		{
			Location.X.SetValueDirectly(0.0f);
			Location.Y.SetValueDirectly(0.0f);
			Location.Z.SetValueDirectly(0.0f);

			Rotation.X.SetValueDirectly(0.0f);
			Rotation.Y.SetValueDirectly(0.0f);
			Rotation.Z.SetValueDirectly(0.0f);

			Scale.X.SetValueDirectly(1.0f);
			Scale.Y.SetValueDirectly(1.0f);
			Scale.Z.SetValueDirectly(1.0f);

			LocationVelocity.X.SetValueDirectly(0.0f);
			LocationVelocity.Y.SetValueDirectly(0.0f);
			LocationVelocity.Z.SetValueDirectly(0.0f);

			RotationVelocity.X.SetValueDirectly(0.0f);
			RotationVelocity.Y.SetValueDirectly(0.0f);
			RotationVelocity.Z.SetValueDirectly(0.0f);

			ScaleVelocity.X.SetValueDirectly(0.0f);
			ScaleVelocity.Y.SetValueDirectly(0.0f);
			ScaleVelocity.Z.SetValueDirectly(0.0f);

			RemovedTime.Infinite.SetValueDirectly(true);
			RemovedTime.Value.SetValueDirectly(0);

			CountX.SetValueDirectly(1);
			CountY.SetValueDirectly(1);
			CountZ.SetValueDirectly(1);

			Distance.SetValueDirectly(5);

			TimeSpan.SetValueDirectly(0);

			ColorAll.R.SetValueDirectly(255);
			ColorAll.G.SetValueDirectly(255);
			ColorAll.B.SetValueDirectly(255);
			ColorAll.A.SetValueDirectly(255);

			TargetLocation.X.SetValueDirectly(0.0f);
			TargetLocation.Y.SetValueDirectly(0.0f);
			TargetLocation.Z.SetValueDirectly(0.0f);
		}
	}
}

namespace EffekseerTool.Data
{
	public class EffectCullingValues
	{
		[Name(language = Language.Japanese, value = "カリングの表示")]
		[Description(language = Language.Japanese, value = "カリングの表示非表示")]
		[Name(language = Language.English, value = "Enable Culling")]
		[Description(language = Language.English, value = "Whether to render hidden surfaces")]
		[Undo(Undo = false)]
		public Value.Boolean IsShown
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "カリングの種類")]
		[Name(language = Language.English, value = "Culling mode")]
		[Selector(ID = 0)]
		public Value.Enum<ParamaterType> Type
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 1)]
		[IO(Export = true)]
		public SphereParamater Sphere
		{
			get;
			private set;
		}

		public class SphereParamater
		{
			[Name(language = Language.Japanese, value = "半径")]
			[Description(language = Language.Japanese, value = "カリング球の半径")]
			[Name(language = Language.English, value = "Radius")]
			[Description(language = Language.English, value = "Radius of spherical culling")]
			[Undo(Undo = true)]
			public Value.Float Radius
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "位置")]
			[Description(language = Language.Japanese, value = "中心の位置")]
			[Name(language = Language.English, value = "Position")]
			[Description(language = Language.English, value = "Central location")]
			[Undo(Undo = false)]
			public Value.Vector3D Location
			{
				get;
				private set;
			}

			public SphereParamater()
			{
				Radius = new Value.Float(1);
				Location = new Value.Vector3D();
			}
		}

		public EffectCullingValues()
		{
			IsShown = new Value.Boolean(false);
			Type = new Value.Enum<ParamaterType>(ParamaterType.None);
			Sphere = new SphereParamater();
		}

		public enum ParamaterType : int
		{
			[Name(value = "なし", language = Language.Japanese)]
			[Name(value = "None", language = Language.English)]
			None = 0,
			[Name(value = "球", language = Language.Japanese)]
			[Name(value = "Spherical", language = Language.English)]
			Sphere = 1,
		}
	}
}

namespace EffekseerTool.Data
{
	public enum AxisType
	{
		[Name(value = "X軸", language = Language.Japanese)]
		[Name(value = "X-Axis", language = Language.English)]
		XAxis,

		[Name(value = "Y軸", language = Language.Japanese)]
		[Name(value = "Y-Axis", language = Language.English)]
		YAxis,

		[Name(value = "Z軸", language = Language.Japanese)]
		[Name(value = "Z-Axis", language = Language.English)]
		ZAxis,
	}

	public class GenerationLocationValues
	{
		[Name(value = "生成角度に影響", language = Language.Japanese)]
		[Name(value = "Set angle on spawn", language = Language.English)]
		public Value.Boolean EffectsRotation
		{
			get;
			private set;
		}

		[Selector(ID = 0)]
		public Value.Enum<ParameterType> Type
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 0)]
		[IO(Export = true)]
		public PointParameter Point
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 1)]
		[IO(Export = true)]
		public SphereParameter Sphere
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 2)]
		[IO(Export = true)]
		public ModelParameter Model
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 3)]
		[IO(Export = true)]
		public CircleParameter Circle
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 4)]
		[IO(Export = true)]
		public LineParameter Line
		{
			get;
			private set;
		}

		internal GenerationLocationValues()
		{
			EffectsRotation = new Value.Boolean(false);
			Type = new Value.Enum<ParameterType>();
			Point = new PointParameter();
			Sphere = new SphereParameter();
			Model = new ModelParameter();
			Circle = new CircleParameter();
			Line = new LineParameter();
		}

		public class PointParameter
		{
			[Name(value = "生成位置", language = Language.Japanese)]
			[Name(value = "Spawn location", language = Language.English)]
			public Value.Vector3DWithRandom Location
			{
				get;
				private set;
			}

			internal PointParameter()
			{
				Location = new Value.Vector3DWithRandom();
			}
		}

		public class LineParameter
		{
			[Name(value = "分割数", language = Language.Japanese)]
			[Name(value = "Verticies", language = Language.English)]
			public Value.Int Division
			{
				get;
				private set;
			}

			[Name(value = "開始位置", language = Language.Japanese)]
			[Name(value = "Init Position", language = Language.English)]
			public Value.Vector3DWithRandom PositionStart
			{
				get;
				private set;
			}

			[Name(value = "終了位置", language = Language.Japanese)]
			[Name(value = "Final Position", language = Language.English)]
			public Value.Vector3DWithRandom PositionEnd
			{
				get;
				private set;
			}

			[Name(value = "位置ノイズ", language = Language.Japanese)]
			[Name(value = "Position Noize", language = Language.English)]
			public Value.FloatWithRandom PositionNoize
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "生成位置種類")]
			[Name(language = Language.English, value = "Spawn Mode")]
			public Value.Enum<LineType> Type
			{
				get;
				private set;
			}

			public LineParameter()
			{
				Division = new Value.Int(8, int.MaxValue, 1);
				PositionStart = new Value.Vector3DWithRandom();
				PositionEnd = new Value.Vector3DWithRandom();
				PositionNoize = new Value.FloatWithRandom();
				Type = new Value.Enum<LineType>(LineType.Random);
			}
		}

		public class SphereParameter
		{
			[Name(value = "半径", language = Language.Japanese)]
			[Name(value = "Radius", language = Language.English)]
			public Value.FloatWithRandom Radius
			{
				get;
				private set;
			}

			[Name(value = "X軸角度(度)", language = Language.Japanese)]
			[Name(value = "X Rotation", language = Language.English)]
			public Value.FloatWithRandom RotationX
			{
				get;
				private set;
			}

			[Name(value = "Y軸角度(度)", language = Language.Japanese)]
			[Name(value = "Y Rotation", language = Language.English)]
			public Value.FloatWithRandom RotationY
			{
				get;
				private set;
			}

			internal SphereParameter()
			{
				Radius = new Value.FloatWithRandom(0.0f, float.MaxValue, 0.0f);
				RotationX = new Value.FloatWithRandom(0.0f);
				RotationY = new Value.FloatWithRandom(0.0f);
			}
		}

		public class ModelParameter
		{
			[Name(language = Language.Japanese, value = "モデル")]
			[Description(language = Language.Japanese, value = "モデルファイル")]
			[Name(language = Language.English, value = "Model")]
			[Description(language = Language.English, value = "Model File")]
			public Value.PathForModel Model
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "生成位置種類")]
			[Name(language = Language.English, value = "Method of Spawning")]
			public Value.Enum<ModelType> Type
			{
				get;
				private set;
			}

			public ModelParameter()
			{
                Model = new Value.PathForModel(Resources.GetString("ModelFilter"), true, "");				
				Type = new Value.Enum<ModelType>(ModelType.Random);
			}
		}

		public class CircleParameter
		{
			[Name(language = Language.Japanese, value = "軸方向")]
			[Name(language = Language.English, value = "Axis Direction")]
			public Value.Enum<AxisType> AxisDirection
			{
				get;
				private set;
			}

			[Name(value = "分割数", language = Language.Japanese)]
			[Name(value = "Verticies", language = Language.English)]
			public Value.Int Division
			{
				get;
				private set;
			}

			[Name(value = "半径", language = Language.Japanese)]
			[Name(value = "Radius", language = Language.English)]
			public Value.FloatWithRandom Radius
			{
				get;
				private set;
			}

			[Name(value = "開始角度", language = Language.Japanese)]
			[Name(value = "Init Angle", language = Language.English)]
			public Value.FloatWithRandom AngleStart
			{
				get;
				private set;
			}

			[Name(value = "終了角度", language = Language.Japanese)]
			[Name(value = "Final Angle", language = Language.English)]
			public Value.FloatWithRandom AngleEnd
			{
				get;
				private set;
			}

			[Name(value = "角度ノイズ", language = Language.Japanese)]
			[Name(value = "Angle Noize", language = Language.English)]
			public Value.FloatWithRandom AngleNoize
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "生成位置種類")]
			[Name(language = Language.English, value = "Spawn Mode")]
			public Value.Enum<CircleType> Type
			{
				get;
				private set;
			}

			public CircleParameter()
			{
				AxisDirection = new Value.Enum<AxisType>(AxisType.ZAxis);
				Division = new Value.Int(8, int.MaxValue, 1);
				Radius = new Value.FloatWithRandom();
				AngleStart = new Value.FloatWithRandom(0, float.MaxValue, float.MinValue);
				AngleEnd = new Value.FloatWithRandom(360, float.MaxValue, float.MinValue);
				AngleNoize = new Value.FloatWithRandom(0);
				Type = new Value.Enum<CircleType>(CircleType.Random);
			}
		}

		public enum ParameterType : int
		{
			[Name(value = "点", language = Language.Japanese)]
			[Name(value = "Point", language = Language.English)]
			Point = 0,
			[Name(value = "線", language = Language.Japanese)]
			[Name(value = "Line", language = Language.English)]
			Line = 4,
			[Name(value = "円", language = Language.Japanese)]
			[Name(value = "Circle", language = Language.English)]
			Circle = 3,
			[Name(value = "球", language = Language.Japanese)]
			[Name(value = "Sphere", language = Language.English)]
			Sphere = 1,
			[Name(value = "モデル", language = Language.Japanese)]
			[Name(value = "Model", language = Language.English)]
			Model = 2,
			
		}

		public enum ModelType : int
		{
			[Name(value = "ランダム", language = Language.Japanese)]
			[Name(value = "Random", language = Language.English)]
			Random = 0,
			[Name(value = "頂点", language = Language.Japanese)]
			[Name(value = "Vertex", language = Language.English)]
			Vertex = 1,
			[Name(value = "頂点(ランダム)", language = Language.Japanese)]
			[Name(value = "Random Vertex", language = Language.English)]
			RandomVertex = 2,
			[Name(value = "面", language = Language.Japanese)]
			[Name(value = "Surface", language = Language.English)]
			Face = 3,
			[Name(value = "面(ランダム)", language = Language.Japanese)]
			[Name(value = "Random Surface", language = Language.English)]
			RandomFace = 4,
		}

		public enum LineType : int
		{
			[Name(value = "ランダム", language = Language.Japanese)]
			[Name(value = "Random", language = Language.English)]
			Random = 0,
			[Name(value = "順番", language = Language.Japanese)]
			[Name(value = "Order", language = Language.English)]
			Order = 1,
		}

		public enum CircleType : int
		{
			[Name(value = "ランダム", language = Language.Japanese)]
			[Name(value = "Random", language = Language.English)]
			Random = 0,
			[Name(value = "正順", language = Language.Japanese)]
			[Name(value = "Clockwise", language = Language.English)]
			Order = 1,
			[Name(value = "逆順", language = Language.Japanese)]
			[Name(value = "Counter", language = Language.English)]
			ReverseOrder = 2,
		}
	}
}

namespace EffekseerTool.Data
{
	public class GlobalValues
	{
		[Name(language = Language.Japanese, value = "ランダムシード")]
		[Description(language = Language.Japanese, value = "ランダムのシード。-1でシード指定なし")]
		[Name(language = Language.English, value = "Random seed")]
		[Description(language = Language.English, value = "Random's seed. When this value is -1, seed is not specified.")]
		public Value.Int RandomSeed
		{
			get;
			private set;
		}

		public GlobalValues()
		{
			RandomSeed = new Value.Int(-1, int.MaxValue - 1);
		}
	}
}

namespace EffekseerTool.Data
{
	public class IO
	{
		public static XmlElement SaveObjectToElement(XmlDocument doc, string element_name, object o, bool isClip)
		{
			XmlElement e_o = doc.CreateElement(element_name);

			var properties = o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var property in properties)
			{
				var io_attribute = property.GetCustomAttributes(typeof(IOAttribute), false).FirstOrDefault() as IOAttribute;
				if (io_attribute != null && !io_attribute.Export) continue;

				var method = typeof(IO).GetMethod("SaveToElement", new Type[] { typeof(XmlDocument), typeof(string), property.PropertyType, typeof(bool) });
				if (method != null)
				{
					var property_value = property.GetValue(o, null);
					var element = method.Invoke(null, new object[] { doc, property.Name, property_value, isClip });

					if (element != null)
					{
						e_o.AppendChild(element as XmlNode);
					}
				}
				else
				{
					if (io_attribute != null && io_attribute.Export)
					{
						var property_value = property.GetValue(o, null);
						var element = SaveObjectToElement(doc, property.Name, property_value, isClip);

						if (element != null && element.ChildNodes.Count > 0)
						{
							e_o.AppendChild(element as XmlNode);
						}
					}
				}
			}

			if(e_o.ChildNodes.Count > 0) return e_o;
			return null;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, NodeBase node, bool isClip)
		{
			return SaveObjectToElement(doc, element_name, node, isClip);
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, NodeBase.ChildrenCollection children, bool isClip)
		{
			var e = doc.CreateElement(element_name);
			for (int i = 0; i < children.Count; i++)
			{
				var e_node = SaveToElement(doc, children[i].GetType().Name, children[i], isClip);
				e.AppendChild(e_node);
			}

			return e;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.String value, bool isClip)
		{
			if (value.DefaultValue == value.Value && !isClip) return null;
			var text = value.GetValue().ToString();
			return doc.CreateTextElement(element_name, text);
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Boolean value, bool isClip)
		{
			if (value.DefaultValue == value.Value && !isClip) return null;
			var text = value.GetValue().ToString();
			return doc.CreateTextElement(element_name, text);
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Int value, bool isClip)
		{
			if (value.Value == value.DefaultValue && !isClip) return null;
			var text = value.GetValue().ToString();
			return doc.CreateTextElement(element_name, text);
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Float value, bool isClip)
		{
			if (value.Value == value.DefaultValue && !isClip) return null;
			var text = value.GetValue().ToString();
			return doc.CreateTextElement(element_name, text);
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.IntWithInifinite value, bool isClip)
		{
			var e = doc.CreateElement(element_name);
			var v = SaveToElement(doc, "Value", value.Value, isClip);
			var i = SaveToElement(doc, "Infinite", value.Infinite, isClip);
			
			if (v != null) e.AppendChild(v);
			if (i != null) e.AppendChild(i);

			return e.ChildNodes.Count > 0 ? e : null;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Vector2D value, bool isClip)
		{
			var e = doc.CreateElement(element_name);
			var x = SaveToElement(doc, "X", value.X, isClip);
			var y = SaveToElement(doc, "Y", value.Y, isClip);

			if (x != null) e.AppendChild(x);
			if (y != null) e.AppendChild(y);

			return e.ChildNodes.Count > 0 ? e : null;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Vector3D value, bool isClip)
		{
			var e = doc.CreateElement(element_name);
			var x = SaveToElement(doc, "X", value.X, isClip);
			var y = SaveToElement(doc, "Y", value.Y, isClip);
			var z = SaveToElement(doc, "Z", value.Z, isClip);

			if (x != null) e.AppendChild(x);
			if (y != null) e.AppendChild(y);
			if (z != null) e.AppendChild(z);

			return e.ChildNodes.Count > 0 ? e : null;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Color value, bool isClip)
		{
			var e = doc.CreateElement(element_name);
			var r = SaveToElement(doc, "R", value.R, isClip);
			var g = SaveToElement(doc, "G", value.G, isClip);
			var b = SaveToElement(doc, "B", value.B, isClip);
			var a = SaveToElement(doc, "A", value.A, isClip);

			if (r != null) e.AppendChild(r);
			if (g != null) e.AppendChild(g);
			if (b != null) e.AppendChild(b);
			if (a != null) e.AppendChild(a);

			return e.ChildNodes.Count > 0 ? e : null;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.IntWithRandom value, bool isClip)
		{
			var e = doc.CreateElement(element_name);
			if (value.DefaultValueCenter != value.Center || isClip) e.AppendChild(doc.CreateTextElement("Center", value.Center.ToString()));
			if (value.DefaultValueMax != value.Max || isClip) e.AppendChild(doc.CreateTextElement("Max", value.Max.ToString()));
			if (value.DefaultValueMin != value.Min || isClip) e.AppendChild(doc.CreateTextElement("Min", value.Min.ToString()));
			if (value.DefaultDrawnAs != value.DrawnAs || isClip) e.AppendChild(doc.CreateTextElement("DrawnAs", (int)value.DrawnAs));

			return e.ChildNodes.Count > 0 ? e : null;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.FloatWithRandom value, bool isClip)
		{
			var e = doc.CreateElement(element_name);

			if (value.DefaultValueCenter != value.Center || isClip) e.AppendChild(doc.CreateTextElement("Center", value.Center.ToString()));
			if (value.DefaultValueMax != value.Max || isClip) e.AppendChild(doc.CreateTextElement("Max", value.Max.ToString()));
			if (value.DefaultValueMin != value.Min || isClip) e.AppendChild(doc.CreateTextElement("Min", value.Min.ToString()));
			if (value.DefaultDrawnAs != value.DrawnAs || isClip) e.AppendChild(doc.CreateTextElement("DrawnAs", (int)value.DrawnAs));
			
			return e.ChildNodes.Count > 0 ? e : null;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Vector2DWithRandom value, bool isClip)
		{
			var e = doc.CreateElement(element_name);
			var x = SaveToElement(doc, "X", value.X, isClip);
			var y = SaveToElement(doc, "Y", value.Y, isClip);
			var da = (value.DefaultDrawnAs != value.DrawnAs || isClip) ? doc.CreateTextElement("DrawnAs", (int)value.DrawnAs) : null;

			if (x != null) e.AppendChild(x);
			if (y != null) e.AppendChild(y);
			if (da != null) e.AppendChild(da);

			return e.ChildNodes.Count > 0 ? e : null;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Vector3DWithRandom value, bool isClip)
		{
			var e = doc.CreateElement(element_name);
			var x = SaveToElement(doc, "X", value.X, isClip);
			var y = SaveToElement(doc, "Y", value.Y, isClip);
			var z = SaveToElement(doc, "Z", value.Z, isClip);
			var da = (value.DefaultDrawnAs != value.DrawnAs || isClip) ? doc.CreateTextElement("DrawnAs", (int)value.DrawnAs) : null;

			if (x != null) e.AppendChild(x);
			if (y != null) e.AppendChild(y);
			if (z != null) e.AppendChild(z);
			if (da != null) e.AppendChild(da);

			return e.ChildNodes.Count > 0 ? e : null;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.ColorWithRandom value, bool isClip)
		{
			var e = doc.CreateElement(element_name);
			var r = SaveToElement(doc, "R", value.R, isClip);
			var g = SaveToElement(doc, "G", value.G, isClip);
			var b = SaveToElement(doc, "B", value.B, isClip);
			var a = SaveToElement(doc, "A", value.A, isClip);
			var da = (value.DefaultDrawnAs != value.DrawnAs || isClip) ? doc.CreateTextElement("DrawnAs", (int)value.DrawnAs) : null;
			var cs = (value.DefaultColorSpace != value.ColorSpace || isClip) ? doc.CreateTextElement("ColorSpace", (int)value.ColorSpace) : null;

			if (r != null) e.AppendChild(r);
			if (g != null) e.AppendChild(g);
			if (b != null) e.AppendChild(b);
			if (a != null) e.AppendChild(a);
			if (da != null) e.AppendChild(da);
			if (cs != null) e.AppendChild(cs);
			return e.ChildNodes.Count > 0 ? e : null;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.EnumBase value, bool isClip)
		{
			if (value.GetValueAsInt() == value.GetDefaultValueAsInt() && !isClip) return null;

			var text = value.GetValueAsInt().ToString();
			return doc.CreateTextElement(element_name, text);
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Path value, bool isClip)
		{
			if (value.DefaultValue == value.GetAbsolutePath() && !isClip) return null;

			var text = "";
			if(!isClip && value.IsRelativeSaved)
				text = value.GetRelativePath();
			else
				text = value.GetAbsolutePath();

			return doc.CreateTextElement(element_name, text);
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.FCurveVector2D value, bool isClip)
		{
			var e = doc.CreateElement(element_name);
			var keys = doc.CreateElement("Keys");
			var x = doc.CreateElement("X");
			var y = doc.CreateElement("Y");

			int index = 0;

			Action<Value.FCurve<float>, XmlElement> setValues = (v, xml) =>
				{
					index = 0;

					var st = SaveToElement(doc, "StartType", v.StartType, isClip);
					var et = SaveToElement(doc, "EndType", v.EndType, isClip);
					var omax = SaveToElement(doc, "OffsetMax", v.OffsetMax, isClip);
					var omin = SaveToElement(doc, "OffsetMin", v.OffsetMin, isClip);
					var s = SaveToElement(doc, "Sampling", v.Sampling, isClip);

					if (st != null) xml.AppendChild(st);
					if (et != null) xml.AppendChild(et);
					if (omax != null) xml.AppendChild(omax);
					if (omin != null) xml.AppendChild(omin);
					if (s != null) xml.AppendChild(s);

					foreach (var k_ in v.Keys)
					{
						var k = doc.CreateElement("Key" + index.ToString());
						k.AppendChild(doc.CreateTextElement("Frame", k_.Frame.ToString()));
						k.AppendChild(doc.CreateTextElement("Value", k_.ValueAsFloat.ToString()));
						k.AppendChild(doc.CreateTextElement("LeftX", k_.LeftX.ToString()));
						k.AppendChild(doc.CreateTextElement("LeftY", k_.LeftY.ToString()));
						k.AppendChild(doc.CreateTextElement("RightX", k_.RightX.ToString()));
						k.AppendChild(doc.CreateTextElement("RightY", k_.RightY.ToString()));

						k.AppendChild(doc.CreateTextElement("InterpolationType", k_.InterpolationType.GetValueAsInt()));

						xml.AppendChild(k);
						index++;
					}
				};

			setValues(value.X, x);
			setValues(value.Y, y);

			if (x.ChildNodes.Count > 0) keys.AppendChild(x);
			if (y.ChildNodes.Count > 0) keys.AppendChild(y);
			if (keys.ChildNodes.Count > 0) e.AppendChild(keys);

			return e.ChildNodes.Count > 0 ? e : null;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.FCurveVector3D value, bool isClip)
		{
			var e = doc.CreateElement(element_name);
			var keys = doc.CreateElement("Keys");
			var x = doc.CreateElement("X");
			var y = doc.CreateElement("Y");
			var z = doc.CreateElement("Z");

			int index = 0;

			Action<Value.FCurve<float>, XmlElement> setValues = (v, xml) =>
			{
				index = 0;

				var st = SaveToElement(doc, "StartType", v.StartType, isClip);
				var et = SaveToElement(doc, "EndType", v.EndType, isClip);
				var omax = SaveToElement(doc, "OffsetMax", v.OffsetMax, isClip);
				var omin = SaveToElement(doc, "OffsetMin", v.OffsetMin, isClip);
				var s = SaveToElement(doc, "Sampling", v.Sampling, isClip);

				if (st != null) xml.AppendChild(st);
				if (et != null) xml.AppendChild(et);
				if (omax != null) xml.AppendChild(omax);
				if (omin != null) xml.AppendChild(omin);
				if (s != null) xml.AppendChild(s);

				foreach (var k_ in v.Keys)
				{
					var k = doc.CreateElement("Key" + index.ToString());
					k.AppendChild(doc.CreateTextElement("Frame", k_.Frame.ToString()));
					k.AppendChild(doc.CreateTextElement("Value", k_.ValueAsFloat.ToString()));
					k.AppendChild(doc.CreateTextElement("LeftX", k_.LeftX.ToString()));
					k.AppendChild(doc.CreateTextElement("LeftY", k_.LeftY.ToString()));
					k.AppendChild(doc.CreateTextElement("RightX", k_.RightX.ToString()));
					k.AppendChild(doc.CreateTextElement("RightY", k_.RightY.ToString()));

					k.AppendChild(doc.CreateTextElement("InterpolationType", k_.InterpolationType.GetValueAsInt()));

					xml.AppendChild(k);
					index++;
				}
			};

			setValues(value.X, x);
			setValues(value.Y, y);
			setValues(value.Z, z);

			if (x.ChildNodes.Count > 0) keys.AppendChild(x);
			if (y.ChildNodes.Count > 0) keys.AppendChild(y);
			if (z.ChildNodes.Count > 0) keys.AppendChild(z);
			if (keys.ChildNodes.Count > 0) e.AppendChild(keys);

			return e.ChildNodes.Count > 0 ? e : null;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.FCurveColorRGBA value, bool isClip)
		{
			var e = doc.CreateElement(element_name);
			var keys = doc.CreateElement("Keys");
			var r = doc.CreateElement("R");
			var g = doc.CreateElement("G");
			var b = doc.CreateElement("B");
			var a = doc.CreateElement("A");

			int index = 0;

			Action<Value.FCurve<byte>, XmlElement> setValues = (v, xml) =>
			{
				index = 0;

				var st = SaveToElement(doc, "StartType", v.StartType, isClip);
				var et = SaveToElement(doc, "EndType", v.EndType, isClip);
				var omax = SaveToElement(doc, "OffsetMax", v.OffsetMax, isClip);
				var omin = SaveToElement(doc, "OffsetMin", v.OffsetMin, isClip);
				var s = SaveToElement(doc, "Sampling", v.Sampling, isClip);

				if (st != null) xml.AppendChild(st);
				if (et != null) xml.AppendChild(et);
				if (omax != null) xml.AppendChild(omax);
				if (omin != null) xml.AppendChild(omin);
				if (s != null) xml.AppendChild(s);

				foreach (var k_ in v.Keys)
				{
					var k = doc.CreateElement("Key" + index.ToString());
					k.AppendChild(doc.CreateTextElement("Frame", k_.Frame.ToString()));
					k.AppendChild(doc.CreateTextElement("Value", k_.ValueAsFloat.ToString()));
					k.AppendChild(doc.CreateTextElement("LeftX", k_.LeftX.ToString()));
					k.AppendChild(doc.CreateTextElement("LeftY", k_.LeftY.ToString()));
					k.AppendChild(doc.CreateTextElement("RightX", k_.RightX.ToString()));
					k.AppendChild(doc.CreateTextElement("RightY", k_.RightY.ToString()));

					k.AppendChild(doc.CreateTextElement("InterpolationType", k_.InterpolationType.GetValueAsInt()));

					xml.AppendChild(k);
					index++;
				}
			};

			setValues(value.R, r);
			setValues(value.G, g);
			setValues(value.B, b);
			setValues(value.A, a);

			if (r.ChildNodes.Count > 0) keys.AppendChild(r);
			if (g.ChildNodes.Count > 0) keys.AppendChild(g);
			if (b.ChildNodes.Count > 0) keys.AppendChild(b);
			if (a.ChildNodes.Count > 0) keys.AppendChild(a);

			if (keys.ChildNodes.Count > 0) e.AppendChild(keys);

			return e.ChildNodes.Count > 0 ? e : null;
		}

		public static void LoadObjectFromElement(XmlElement e, ref object o, bool isClip)
		{
			var o_type = o.GetType();

			foreach (var _ch_node in e.ChildNodes)
			{
				var ch_node = _ch_node as XmlElement;
				var local_name = ch_node.LocalName;

				var property = o_type.GetProperty(local_name);
				if (property == null) continue;

				var io_attribute = property.GetCustomAttributes(typeof(IOAttribute), false).FirstOrDefault() as IOAttribute;
				if (io_attribute != null && !io_attribute.Import) continue;

				var method = typeof(IO).GetMethod("LoadFromElement", new Type[] { typeof(XmlElement), property.PropertyType, typeof(bool) });
				if (method != null)
				{
					var property_value = property.GetValue(o, null);
					method.Invoke(null, new object[] { ch_node, property_value, isClip });
				}
				else
				{
					if (io_attribute != null && io_attribute.Import)
					{
						var property_value = property.GetValue(o, null);
						LoadObjectFromElement(ch_node, ref property_value, isClip);
					}
				}
			}
		}

		public static void LoadFromElement(XmlElement e, NodeBase node, bool isClip)
		{
			var o = node as object;
			LoadObjectFromElement(e, ref o, isClip);
		}

		public static void LoadFromElement(XmlElement e, NodeBase.ChildrenCollection children, bool isClip)
		{
			children.Node.ClearChildren();

			for (var i = 0; i < e.ChildNodes.Count; i++)
			{
				var e_child = e.ChildNodes[i] as XmlElement;
				if (e_child.LocalName != "Node") continue;

				var node = children.Node.AddChild();
				LoadFromElement(e_child, node, isClip);
			}
		}

		public static void LoadFromElement(XmlElement e, Value.String value, bool isClip)
		{
			var text = e.GetText();
			value.SetValue(text);
		}

		public static void LoadFromElement(XmlElement e, Value.Boolean value, bool isClip)
		{
			var text = e.GetText();
			var parsed = false;
			if (bool.TryParse(text, out parsed))
			{
				value.SetValue(parsed);
			}
		}

		public static void LoadFromElement(XmlElement e, Value.Int value, bool isClip)
		{
			var text = e.GetText();
			var parsed = 0;
			if (int.TryParse(text, System.Globalization.NumberStyles.Integer, Setting.NFI, out parsed))
			{
				value.SetValue(parsed);
			}
		}

		public static void LoadFromElement(XmlElement e, Value.Float value, bool isClip)
		{
			var text = e.GetText();
			var parsed = 0.0f;
			if (float.TryParse(text, System.Globalization.NumberStyles.Float, Setting.NFI, out parsed))
			{
				value.SetValue(parsed);
			}
		}

		public static void LoadFromElement(XmlElement e, Value.IntWithInifinite value, bool isClip)
		{
			var e_value = e["Value"] as XmlElement;
			var e_infinite = e["Infinite"] as XmlElement;

			// Convert int into intWithInfinit
			if(e_value == null && e_infinite == null)
			{
				var i = e.GetTextAsInt();
				value.Value.SetValue(i);
			}
			else
			{
				if (e_value != null) LoadFromElement(e_value, value.Value, isClip);
				if (e_infinite != null) LoadFromElement(e_infinite, value.Infinite, isClip);
			}
		}

		public static void LoadFromElement(XmlElement e, Value.Vector2D value, bool isClip)
		{
			var e_x = e["X"] as XmlElement;
			var e_y = e["Y"] as XmlElement;

			if (e_x != null) LoadFromElement(e_x, value.X, isClip);
			if (e_y != null) LoadFromElement(e_y, value.Y, isClip);
		}

		public static void LoadFromElement(XmlElement e, Value.Vector3D value, bool isClip)
		{
			var e_x = e["X"] as XmlElement;
			var e_y = e["Y"] as XmlElement;
			var e_z = e["Z"] as XmlElement;

			if (e_x != null) LoadFromElement(e_x, value.X, isClip);
			if (e_y != null) LoadFromElement(e_y, value.Y, isClip);
			if (e_z != null) LoadFromElement(e_z, value.Z, isClip);
		}

		public static void LoadFromElement(XmlElement e, Value.Color value, bool isClip)
		{
			var e_r = e["R"] as XmlElement;
			var e_g = e["G"] as XmlElement;
			var e_b = e["B"] as XmlElement;
			var e_a = e["A"] as XmlElement;

			if (e_r != null) LoadFromElement(e_r, value.R, isClip);
			if (e_g != null) LoadFromElement(e_g, value.G, isClip);
			if (e_b != null) LoadFromElement(e_b, value.B, isClip);
			if (e_a != null) LoadFromElement(e_a, value.A, isClip);
		}

		public static void LoadFromElement(XmlElement e, Value.IntWithRandom value, bool isClip)
		{
			var e_c = e["Center"];
			var e_max = e["Max"];
			var e_min = e["Min"];
			var e_da = e["DrawnAs"];

			if (e_c != null)
			{
				var center = e_c.GetTextAsInt();
				value.SetCenter(center);
			}

			if (e_max != null)
			{
				var max = e_max.GetTextAsInt();
				value.SetMax(max);
			}
			else
			{
				value.SetMax(value.DefaultValueMax);
			}

			if (e_min != null)
			{
				var min = e_min.GetTextAsInt();
				value.SetMin(min);
			}
			else
			{
				value.SetMin(value.DefaultValueMin);
			}

			if (e_da != null)
			{
				value.DrawnAs = (DrawnAs)e_da.GetTextAsInt();
			}
		}

		public static void LoadFromElement(XmlElement e, Value.FloatWithRandom value, bool isClip)
		{
			var e_c = e["Center"];
			var e_max = e["Max"];
			var e_min = e["Min"];
			var e_da = e["DrawnAs"];

			if (e_c != null)
			{
				var center = e_c.GetTextAsFloat();
				value.SetCenter(center);
			}

			if (e_max != null)
			{
				var max = e_max.GetTextAsFloat();
				value.SetMax(max);
			}
			else
			{
				value.SetMax(value.DefaultValueMax);
			}

			if (e_min != null)
			{
				var min = e_min.GetTextAsFloat();
				value.SetMin(min);
			}
			else
			{
				value.SetMin(value.DefaultValueMin);
			}

			if (e_da != null)
			{
				value.DrawnAs = (DrawnAs)e_da.GetTextAsInt();
			}
		}

		public static void LoadFromElement(XmlElement e, Value.Vector2DWithRandom value, bool isClip)
		{
			var e_x = e["X"] as XmlElement;
			var e_y = e["Y"] as XmlElement;
			var e_da = e["DrawnAs"];

			// Convert Vector2D into Vector2DWithRandom
			if(e_x != null)
			{
				if(e_da == null && e_x["Max"] == null && e_x["Min"] == null && e_x["Center"] == null)
				{
					var x = e_x.GetTextAsFloat();
					value.X.SetCenter(x);
				}
				else
				{
					LoadFromElement(e_x, value.X, isClip);
				}
			}

			if (e_y != null)
			{
				if (e_da == null && e_y["Max"] == null && e_y["Min"] == null && e_y["Center"] == null)
				{
					var y = e_y.GetTextAsFloat();
					value.Y.SetCenter(y);
				}
				else
				{
					LoadFromElement(e_y, value.Y, isClip);
				}
			}

			if (e_da != null)
			{
				value.DrawnAs = (DrawnAs)e_da.GetTextAsInt();
			}
		}

		public static void LoadFromElement(XmlElement e, Value.Vector3DWithRandom value, bool isClip)
		{
			var e_x = e["X"] as XmlElement;
			var e_y = e["Y"] as XmlElement;
			var e_z = e["Z"] as XmlElement;
			var e_da = e["DrawnAs"];

			if (e_x != null) LoadFromElement(e_x, value.X, isClip);
			if (e_y != null) LoadFromElement(e_y, value.Y, isClip);
			if (e_z != null) LoadFromElement(e_z, value.Z, isClip);

			if (e_da != null)
			{
				value.DrawnAs = (DrawnAs)e_da.GetTextAsInt();
			}
		}

		public static void LoadFromElement(XmlElement e, Value.ColorWithRandom value, bool isClip)
		{
			var e_r = e["R"] as XmlElement;
			var e_g = e["G"] as XmlElement;
			var e_b = e["B"] as XmlElement;
			var e_a = e["A"] as XmlElement;
			var e_da = e["DrawnAs"];
			var e_cs = e["ColorSpace"];

			if (e_r != null) LoadFromElement(e_r, value.R, isClip);
			if (e_g != null) LoadFromElement(e_g, value.G, isClip);
			if (e_b != null) LoadFromElement(e_b, value.B, isClip);
			if (e_a != null) LoadFromElement(e_a, value.A, isClip);

			if (e_da != null)
			{
				value.DrawnAs = (DrawnAs)e_da.GetTextAsInt();
			}

			if (e_cs != null)
			{
				value.SetColorSpace((ColorSpace)e_cs.GetTextAsInt(), false, false);
			}
		}

		public static void LoadFromElement(XmlElement e, Value.Path value, bool isClip)
		{
			var text = e.GetText();

			if (!isClip)
				value.SetRelativePath(text);
			else
				value.SetAbsolutePath(text);
		}

		public static void LoadFromElement(XmlElement e, Value.EnumBase value, bool isClip)
		{
			var text = e.GetText();
			var parsed = 0;
			if (int.TryParse(text, out parsed))
			{
				value.SetValue(parsed);
			}
		}

		public static void LoadFromElement(XmlElement e, Value.FCurveVector2D value, bool isClip)
		{
			var e_keys = e["Keys"] as XmlElement;
			if (e_keys == null) return;

			var e_x = e_keys["X"] as XmlElement;
			var e_y = e_keys["Y"] as XmlElement;

			Action<Data.Value.FCurve<float>, XmlElement> import = (v_, e_) =>
			{
				foreach (XmlElement r in e_.ChildNodes)
				{
					if (r.Name.StartsWith("Key"))
					{
						var f = r.GetTextAsInt("Frame");
						var v = r.GetTextAsFloat("Value");
						var lx = r.GetTextAsFloat("LeftX");
						var ly = r.GetTextAsFloat("LeftY");
						var rx = r.GetTextAsFloat("RightX");
						var ry = r.GetTextAsFloat("RightY");
						var i = r.GetTextAsInt("InterpolationType");
						var s = r.GetTextAsInt("Sampling");

						var t = new Value.FCurveKey<float>(f, v);
						t.SetLeftDirectly(lx, ly);
						t.SetRightDirectly(rx, ry);
						t.InterpolationType.SetValue(i);

						v_.AddKeyDirectly(t);
					}
					else if (r.Name.StartsWith("StartType"))
					{
						var v = r.GetTextAsInt();
						v_.StartType.SetValue(v);
					}
					else if (r.Name.StartsWith("EndType"))
					{
						var v = r.GetTextAsInt();
						v_.EndType.SetValue(v);
					}
					else if (r.Name.StartsWith("OffsetMax"))
					{
						var v = r.GetTextAsFloat();
						v_.OffsetMax.SetValueDirectly(v);
					}
					else if (r.Name.StartsWith("OffsetMin"))
					{
						var v = r.GetTextAsFloat();
						v_.OffsetMin.SetValueDirectly(v);
					}
					else if (r.Name.StartsWith("Sampling"))
					{
						var v = r.GetTextAsInt();
						v_.Sampling.SetValueDirectly(v);
					}
				}
			};

			if (e_x != null) import(value.X, e_x);
			if (e_y != null) import(value.Y, e_y);
		}

		public static void LoadFromElement(XmlElement e, Value.FCurveVector3D value, bool isClip)
		{
			var e_keys = e["Keys"] as XmlElement;
			if (e_keys == null) return;

			var e_x = e_keys["X"] as XmlElement;
			var e_y = e_keys["Y"] as XmlElement;
			var e_z = e_keys["Z"] as XmlElement;

			Action<Data.Value.FCurve<float>, XmlElement> import = (v_, e_) =>
			{
				foreach (XmlElement r in e_.ChildNodes)
				{
					if (r.Name.StartsWith("Key"))
					{
						var f = r.GetTextAsInt("Frame");
						var v = r.GetTextAsFloat("Value");
						var lx = r.GetTextAsFloat("LeftX");
						var ly = r.GetTextAsFloat("LeftY");
						var rx = r.GetTextAsFloat("RightX");
						var ry = r.GetTextAsFloat("RightY");
						var i = r.GetTextAsInt("InterpolationType");

						var t = new Value.FCurveKey<float>(f, v);
						t.SetLeftDirectly(lx, ly);
						t.SetRightDirectly(rx, ry);
						t.InterpolationType.SetValue(i);

						v_.AddKeyDirectly(t);
					}
					else if (r.Name.StartsWith("StartType"))
					{
						var v = r.GetTextAsInt();
						v_.StartType.SetValue(v);
					}
					else if (r.Name.StartsWith("EndType"))
					{
						var v = r.GetTextAsInt();
						v_.EndType.SetValue(v);
					}
					else if (r.Name.StartsWith("OffsetMax"))
					{
						var v = r.GetTextAsFloat();
						v_.OffsetMax.SetValueDirectly(v);
					}
					else if (r.Name.StartsWith("OffsetMin"))
					{
						var v = r.GetTextAsFloat();
						v_.OffsetMin.SetValueDirectly(v);
					}
					else if (r.Name.StartsWith("Sampling"))
					{
						var v = r.GetTextAsInt();
						v_.Sampling.SetValueDirectly(v);
					}
				}
			};

			if (e_x != null) import(value.X, e_x);
			if (e_y != null) import(value.Y, e_y);
			if (e_z != null) import(value.Z, e_z);
		}

		public static void LoadFromElement(XmlElement e, Value.FCurveColorRGBA value, bool isClip)
		{
			Action<Data.Value.FCurve<byte>, XmlElement> import = (v_, e_) =>
				{
					foreach (XmlElement r in e_.ChildNodes)
					{
						if (r.Name.StartsWith("Key"))
						{
							var f = r.GetTextAsInt("Frame");
							var v = r.GetTextAsFloat("Value");
							var lx = r.GetTextAsFloat("LeftX");
							var ly = r.GetTextAsFloat("LeftY");
							var rx = r.GetTextAsFloat("RightX");
							var ry = r.GetTextAsFloat("RightY");
							var i = r.GetTextAsInt("InterpolationType");

							var t = new Value.FCurveKey<byte>(f, (byte)v);
							t.SetLeftDirectly(lx, ly);
							t.SetRightDirectly(rx, ry);
							t.InterpolationType.SetValue(i);

							v_.AddKeyDirectly(t);
						}
						else if (r.Name.StartsWith("StartType"))
						{
							var v = r.GetTextAsInt();
							v_.StartType.SetValue(v);
						}
						else if (r.Name.StartsWith("EndType"))
						{
							var v = r.GetTextAsInt();
							v_.EndType.SetValue(v);
						}
						else if (r.Name.StartsWith("OffsetMax"))
						{
							var v = r.GetTextAsFloat();
							v_.OffsetMax.SetValueDirectly(v);
						}
						else if (r.Name.StartsWith("OffsetMin"))
						{
							var v = r.GetTextAsFloat();
							v_.OffsetMin.SetValueDirectly(v);
						}
						else if (r.Name.StartsWith("Sampling"))
						{
							var v = r.GetTextAsInt();
							v_.Sampling.SetValueDirectly(v);
						}
					}
				};


			var e_keys = e["Keys"] as XmlElement;
			if (e_keys == null) return;

			var e_r = e_keys["R"] as XmlElement;
			var e_g = e_keys["G"] as XmlElement;
			var e_b = e_keys["B"] as XmlElement;
			var e_a = e_keys["A"] as XmlElement;

			if (e_r != null) import(value.R, e_r);
			if (e_g != null) import(value.G, e_g);
			if (e_b != null) import(value.B, e_b);
			if (e_a != null) import(value.A, e_a);
		}
	}
}

namespace EffekseerTool.Data
{
	public class LocationAbsValues
	{
		[Selector(ID = 0)]
		public Value.Enum<ParamaterType> Type
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 0)]
		[IO(Export = true)]
		public NoneParamater None
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 1)]
		[IO(Export = true)]
		public GravityParamater Gravity
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 2)]
		[IO(Export = true)]
		public AttractiveForceParamater AttractiveForce
		{
			get;
			private set;
		}

		internal LocationAbsValues()
		{
			Type = new Value.Enum<ParamaterType>(ParamaterType.None);
			None = new NoneParamater();
			Gravity = new GravityParamater();
			AttractiveForce = new AttractiveForceParamater();
		}

		public class NoneParamater
		{
			internal NoneParamater()
			{
			}
		}

		public class GravityParamater
		{
			[Name(language = Language.Japanese, value = "重力")]
			[Description(language = Language.Japanese, value = "インスタンスにかかる重力")]
			[Name(language = Language.English, value = "Gravity")]
			[Description(language = Language.English, value = "Gravity's effect on the instance")]
			public Value.Vector3D Gravity
			{
				get;
				private set;
			}

			internal GravityParamater()
			{
				Gravity = new Value.Vector3D(0, 0, 0);
			}
		}
		
		public class AttractiveForceParamater
		{
			[Name(language = Language.Japanese, value = "引力")]
			[Description(language = Language.Japanese, value = "ターゲットの引力")]
			[Name(language = Language.English, value = "Attraction")]
			[Description(language = Language.English, value = "Strength of the point of attraction")]
			public Value.Float Force
			{
				get;
				private set;
			}
			
			[Name(language = Language.Japanese, value = "制御")]
			[Description(language = Language.Japanese, value = "移動方向をターゲット方向へ補正量")]
			[Name(language = Language.English, value = "Resistance")]
			[Description(language = Language.English, value = "Resistance to the directional pull of the attractor")]
			public Value.Float Control
			{
				get;
				private set;
			}
			
			[Name(language = Language.Japanese, value = "最小範囲")]
			[Description(language = Language.Japanese, value = "この範囲以内では引力がフルに掛かります")]
			[Name(language = Language.English, value = "Minimum Range")]
			[Description(language = Language.English, value = "Within this range, it will be affected by the attractor")]
			public Value.Float MinRange
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "最大範囲")]
			[Description(language = Language.Japanese, value = "この範囲以外では引力が掛かりません")]
			[Name(language = Language.English, value = "Maximum Range")]
			[Description(language = Language.English, value = "Outside this range, the attractor will have no effect on the instance")]
			public Value.Float MaxRange
			{
				get;
				private set;
			}

			internal AttractiveForceParamater()
			{
				Force = new Value.Float(0.0f, float.MaxValue, float.MinValue, 0.01f);
				Control = new Value.Float(1.0f, float.MaxValue, float.MinValue, 0.1f);
				MinRange = new Value.Float(0.0f, 1000.0f, 0.0f, 1.0f);
				MaxRange = new Value.Float(0.0f, 1000.0f, 0.0f, 1.0f);
			}
		}
	
		public enum ParamaterType : int
		{
			[Name(value = "無し", language = Language.Japanese)]
			[Name(value = "None", language = Language.English)]
			None = 0,
			[Name(value = "重力", language = Language.Japanese)]
			[Name(value = "Gravity", language = Language.English)]
			Gravity = 1,
			[Name(value = "引力(ターゲット有り)", language = Language.Japanese)]
			[Name(value = "Attraction (if point is set)", language = Language.English)]
			AttractiveForce = 2,
		}
	}
}

namespace EffekseerTool.Data
{
	public class LocationValues
	{
		[Selector(ID = 0)]
		public Value.Enum<ParamaterType> Type
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 0)]
		[IO(Export = true)]
		public FixedParamater Fixed
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 1)]
		[IO(Export = true)]
		public PVAParamater PVA
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 2)]
		[IO(Export = true)]
		public Vector3DEasingParamater Easing
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 3)]
		[IO(Export = true)]
		public Vector3DFCurveParameter LocationFCurve
		{
			get;
			private set;
		}

		internal LocationValues()
		{
			Type = new Value.Enum<ParamaterType>(ParamaterType.Fixed);
			Fixed = new FixedParamater();
			PVA = new PVAParamater();
			Easing = new Vector3DEasingParamater();
			LocationFCurve = new Vector3DFCurveParameter();
		}

		public class FixedParamater
		{
			[Name(language = Language.Japanese, value = "位置")]
			[Description(language = Language.Japanese, value = "インスタンスの位置")]
			[Name(language = Language.English, value = "Location")]
			[Description(language = Language.English, value = "Position of the instance")]
			public Value.Vector3D Location
			{
				get;
				private set;
			}

			internal FixedParamater()
			{
				Location = new Value.Vector3D(0, 0, 0);
			}
		}

		public class PVAParamater
		{
			[Name(language = Language.Japanese, value = "位置")]
			[Description(language = Language.Japanese, value = "インスタンスの初期位置")]
			[Name(language = Language.English, value = "Pos")]
			[Description(language = Language.English, value = "Position of the instance")]
			public Value.Vector3DWithRandom Location
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "速度")]
			[Description(language = Language.Japanese, value = "インスタンスの初期速度")]
			[Name(language = Language.English, value = "Speed")]
			[Description(language = Language.English, value = "Initial velocity of the instance")]
			public Value.Vector3DWithRandom Velocity
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "加速度")]
			[Description(language = Language.Japanese, value = "インスタンスの初期加速度")]
			[Name(language = Language.English, value = "Accel")]
			[Description(language = Language.English, value = "Acceleration of the instance")]
			public Value.Vector3DWithRandom Acceleration
			{
				get;
				private set;
			}

			internal PVAParamater()
			{
				Location = new Value.Vector3DWithRandom(0, 0, 0);
				Velocity = new Value.Vector3DWithRandom(0, 0, 0);
				Acceleration = new Value.Vector3DWithRandom(0, 0, 0);
			}
		}

		public enum ParamaterType : int
		{
			[Name(value = "位置", language = Language.Japanese)]
			[Name(value = "Set Position", language = Language.English)]
			Fixed = 0,
			[Name(value = "位置・速度・加速度", language = Language.Japanese)]
			[Name(value = "PVA", language = Language.English)]
			PVA = 1,
			[Name(value = "イージング", language = Language.Japanese)]
			[Name(value = "Easing", language = Language.English)]
			Easing = 2,
			[Name(value = "位置(Fカーブ)", language = Language.Japanese)]
			[Name(value = "F-Curve", language = Language.English)]
			LocationFCurve = 3,
		}
	}
}

namespace EffekseerTool.Data
{
	public class Node : NodeBase
	{

		[IO(Export = true)]
		public CommonValues CommonValues
		{
			get;
			private set;
		}

		[IO(Export = true)]
		public LocationValues LocationValues
		{
			get;
			private set;
		}

		[IO(Export = true)]
		public RotationValues RotationValues
		{
			get;
			private set;
		}

		[IO(Export = true)]
		public ScaleValues ScalingValues
		{
			get;
			private set;
		}

		[IO(Export = true)]
		public LocationAbsValues LocationAbsValues
		{
			get;
			private set;
		}

		[IO(Export = true)]
		public GenerationLocationValues GenerationLocationValues
		{
			get;
			private set;
		}

        [IO(Export = true)]
        public DepthValues DepthValues
        {
            get;
            private set;
        }

        [IO(Export = true)]
		public RendererCommonValues RendererCommonValues
		{
			get;
			private set;
		}

		[IO(Export = true)]
		public RendererValues DrawingValues
		{
			get;
			private set;
		}

        [IO(Export = true)]
        public SoundValues SoundValues
        {
            get;
            private set;
        }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal Node(NodeBase parent)
			:base(parent)
		{
			Name.SetValueDirectly("Node");
			CommonValues = new Data.CommonValues();
			LocationValues = new Data.LocationValues();
			RotationValues = new Data.RotationValues();
			ScalingValues = new Data.ScaleValues();
			LocationAbsValues = new Data.LocationAbsValues();
			GenerationLocationValues = new Data.GenerationLocationValues();
            DepthValues = new DepthValues();
			RendererCommonValues = new Data.RendererCommonValues();
            DrawingValues = new RendererValues();
            SoundValues = new SoundValues();
		}
	}
}

namespace EffekseerTool.Data
{
	public class NodeBase
	{
		List<Node> children = new List<Node>();

		[Name(language = Language.Japanese, value = "描画")]
		[Description(language = Language.Japanese, value = "編集画面にインスタンスを描画するかどうか。\n最終的に出力される結果には関係ない。")]
		[Name(language = Language.English, value = "Visibility")]
		[Description(language = Language.English, value = "Whether to draw the instance to the viewport.\nHas nothing to do with the final output.")]
		public Value.Boolean IsRendered
		{
			get;
			private set;
		}

		[Name(language=Language.Japanese, value="名称")]
		[Description(language=Language.Japanese, value="ノードの名称。\n描画には関係ない。")]
		[Name(language = Language.English, value = "Name")]
		[Description(language = Language.English, value = "The name of the node.")]
		public Value.String Name
		{
			get;
			private set;
		}

		[IO(Export=false)]
		public NodeBase Parent
		{
			get;
			private set;
		}

		public ChildrenCollection Children
		{
			get;
			private set;
		}

		public event ChangedValueEventHandler OnAfterAddNode;

		public event ChangedValueEventHandler OnAfterRemoveNode;

		public event ChangedValueEventHandler OnAfterExchangeNodes;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal NodeBase(NodeBase parent)
		{
			Parent = parent;
			Name = new Value.String("Node");
			IsRendered = new Value.Boolean(true);
			Children = new ChildrenCollection(this);
		}

		/// <summary>
		/// Add child
		/// </summary>
		/// <param name="node">Added node (if null, generated automatically)</param>
		/// <param name="index">inserted position</param>
		/// <returns></returns>
		public Node AddChild(Node node = null, int index = int.MaxValue)
		{
			if(node == null)
			{
				node = new Node(this);
			}

			var old_parent = node.Parent;
			var new_parent = this;
			var old_value = children;
			var new_value = new List<Node>(children);

			if(index == int.MaxValue)
			{
				new_value.Add(node);
			}
			else
			{
				if(index >= children.Count)
				{
					new_value.Add(node);
				}
				else
				{
					new_value.Insert(index, node);
				}
			}

			var cmd = new Command.DelegateCommand(
				() =>
				{
					children = new_value;
					node.Parent = new_parent;

					if (OnAfterAddNode != null)
					{
						OnAfterAddNode(this, new ChangedValueEventArgs(node, ChangedValueType.Execute));
					}
				},
				() =>
				{
					children = old_value;
					node.Parent = old_parent;

					if (OnAfterRemoveNode != null)
					{
						OnAfterRemoveNode(this, new ChangedValueEventArgs(node, ChangedValueType.Unexecute));
					}
				});

			Command.CommandManager.Execute(cmd);

			return node;
		}

		/// <summary>
		/// 子ノードの破棄
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public bool RemoveChild(Node node)
		{
			if (!children.Contains(node)) return false;

			var old_value = children;
			var new_value = new List<Node>(children);
			new_value.Remove(node);

			var cmd = new Command.DelegateCommand(
				() =>
				{
					children = new_value;

					if (OnAfterRemoveNode != null)
					{
						OnAfterRemoveNode(this, new ChangedValueEventArgs(node, ChangedValueType.Execute));
					}
				},
				() =>
				{
					children = old_value;

					if (OnAfterAddNode != null)
					{
						OnAfterAddNode(this, new ChangedValueEventArgs(node, ChangedValueType.Unexecute));
					}
				});

			Command.CommandManager.Execute(cmd);

			if (node == Core.SelectedNode)
			{
				Core.SelectedNode = null;
			}

			return true;
		}

		/// <summary>
		/// 現在のノードの親とノードの間にノードを挿入
		/// </summary>
		public Node InsertParent()
		{
			if (Parent == null) return null;

			var node = new Node(Parent);
			node.children.Add((Node)this);

			var old_value = Parent.children;
			var new_value = new List<Node>(Parent.children);
			for (int i = 0; i < new_value.Count; i++)
			{
				if (new_value[i] == this)
				{
					new_value[i] = node;
					break;
				}
			}

			var parent = Parent;
			var _this = this;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					parent.children = new_value;
					_this.Parent = node;
					if (parent.OnAfterRemoveNode != null)
					{
						parent.OnAfterRemoveNode(parent, new ChangedValueEventArgs(_this, ChangedValueType.Execute));
					}

					if (parent.OnAfterAddNode != null)
					{
						parent.OnAfterAddNode(parent, new ChangedValueEventArgs(node, ChangedValueType.Execute));
					}

				},
				() =>
				{
					parent.children = old_value;
					_this.Parent = parent;
					if (parent.OnAfterAddNode != null)
					{
						parent.OnAfterAddNode(this, new ChangedValueEventArgs(_this, ChangedValueType.Unexecute));
					}

					if (parent.OnAfterRemoveNode != null)
					{
						parent.OnAfterRemoveNode(parent, new ChangedValueEventArgs(node, ChangedValueType.Unexecute));
					}
				});

			Command.CommandManager.Execute(cmd);

			return node;
		}

		/// <summary>
		/// 子ノードの全破棄
		/// </summary>
		public void ClearChildren()
		{
			Command.CommandManager.StartCollection();

			while (children.Count > 0)
			{
				RemoveChild(children[0]);
			}

			Command.CommandManager.EndCollection();
		}

		/// <summary>
		/// Exchange children each other
		/// </summary>
		/// <param name="node1"></param>
		/// <param name="node2"></param>
		/// <returns></returns>
		public bool ExchangeChildren(Node node1, Node node2)
		{
			if (node1 == node2) return false;
			if (!children.Contains(node1)) return false;
			if (!children.Contains(node2)) return false;

			var old_value = children;
			var new_value = new List<Node>(children);
			var ind1 = new_value.FindIndex((n) => { return n == node1; });
			var ind2 = new_value.FindIndex((n) => { return n == node2; });
			new_value[ind1] = node2;
			new_value[ind2] = node1;

			

			var cmd = new Command.DelegateCommand(
				() =>
				{
					Tuple35<Node, Node> nodes = new Tuple35<Node, Node>(node1, node2);
					children = new_value;

					if (OnAfterExchangeNodes != null)
					{
						OnAfterExchangeNodes(this, new ChangedValueEventArgs(nodes, ChangedValueType.Execute));
					}
				},
				() =>
				{
					Tuple35<Node, Node> nodes = new Tuple35<Node, Node>(node2, node1);
					children = old_value;

					if (OnAfterExchangeNodes != null)
					{
						OnAfterExchangeNodes(this, new ChangedValueEventArgs(nodes, ChangedValueType.Unexecute));
					}
				});

			Command.CommandManager.Execute(cmd);

			return true;
		}

		/// <summary>
		/// ノードを子として保有しているか取得
		/// </summary>
		/// <param name="node">ノード</param>
		/// <param name="recursion">再帰的に検索するか</param>
		/// <returns></returns>
		public bool HasNode(NodeBase node, bool recursion)
		{
			for (int i = 0; i < Children.Count; i++)
			{
				if (Children[i] == node) return true;

				if (recursion)
				{
					if (Children[i].HasNode(node, true)) return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 子ノード群
		/// </summary>
		public class ChildrenCollection
		{
			NodeBase _node = null;

			public NodeBase Node
			{
				get
				{
					return _node;
				}
			}

			internal ChildrenCollection(NodeBase node)
			{
				_node = node; 
			}

			internal void SetChildren(List<Node> children)
			{
				_node.children = children;
				foreach (var child in _node.children)
				{
					child.Parent = _node;
				}
			}

			public int Count
			{
				get
				{
					return _node.children.Count;
				}
			}

			public Node this[int index]
			{
				get
				{
					if (Count > index)
					{
						return _node.children[index];
					}
					return null;
				}
			}

			public List<Node> Internal
			{
				get
				{
					return _node.children;
				}
			}
		}
	}
}

namespace EffekseerTool.Data
{
	public class NodeRoot : NodeBase
	{
		internal NodeRoot()
			:base(null)
		{
			Name.SetValueDirectly("Root");
		}
	}
}

namespace EffekseerTool.Data
{
	public class OptionValues
	{
		[Name(language = Language.Japanese, value = "描画モード")]
		[Description(language = Language.Japanese, value = "エフェクトの通常モードの設定")]
		[Name(language = Language.English, value = "Render Mode")]
		[Description(language = Language.English, value = "Set the Render Mode of effects")]
		[Undo(Undo = false)]
		public Value.Enum<RenderMode> RenderingMode
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "グリッド色")]
		[Description(language = Language.Japanese, value = "グリッド色")]
		[Name(language = Language.English, value = "Grid Color")]
		[Description(language = Language.English, value = "Color of the grid")]
		[Undo(Undo = false)]
		public Value.Color GridColor
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "グリッドの表示")]
		[Description(language = Language.Japanese, value = "グリッドの表示非表示")]
		[Name(language = Language.English, value = "Grid Visibility")]
		[Description(language = Language.English, value = "Toggle the visibility of the grid")]
		[Undo(Undo = false)]
		public Value.Boolean IsGridShown
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "XYグリッドの表示")]
		[Description(language = Language.Japanese, value = "XYグリッドの表示非表示")]
		[Name(language = Language.English, value = "X-Y Grid Visibility")]
		[Description(language = Language.English, value = "Toggle the visibility of the grid along the X-Y axes")]
		[Undo(Undo = false)]
		public Value.Boolean IsXYGridShown
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "XZグリッドの表示")]
		[Description(language = Language.Japanese, value = "XZグリッドの表示非表示")]
		[Name(language = Language.English, value = "X-Z Grid Visibility")]
		[Description(language = Language.English, value = "Toggle the visibility of the grid along the X-Z axes")]
		[Undo(Undo = false)]
		public Value.Boolean IsXZGridShown
		{
			get;
			private set;
		}


		[Name(language = Language.Japanese, value = "YZグリッドの表示")]
		[Description(language = Language.Japanese, value = "YZグリッドの表示非表示")]
		[Name(language = Language.English, value = "Y-Z Grid Visibility")]
		[Description(language = Language.English, value = "Toggle the visibility of the grid along the Y-Z axes")]
		[Undo(Undo = false)]
		public Value.Boolean IsYZGridShown
		{
			get;
			private set;
		}


		[Name(language = Language.Japanese, value = "グリッドサイズ")]
		[Description(language = Language.Japanese, value = "表示しているグリッドの幅")]
		[Name(language = Language.English, value = "Grid Size")]
		[Description(language = Language.English, value = "Dimensions of the displayed grid")]
		[Undo(Undo = false)]
		public Value.Float GridLength
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "ライト方向")]
		[Description(language = Language.Japanese, value = "ディレクショナルライトの向き")]
		[Name(language = Language.English, value = "Light Direction")]
		[Description(language = Language.English, value = "Orientation of the directional light")]
		[Undo(Undo = false)]
		public Value.Vector3D LightDirection
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "ライト色")]
		[Description(language = Language.Japanese, value = "ライトのディフュージョン色")]
		[Name(language = Language.English, value = "Light Color")]
		[Description(language = Language.English, value = "Diffuse color of the light")]
		[Undo(Undo = false)]
		public Value.Color LightColor
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "アンビエント色")]
		[Description(language = Language.Japanese, value = "ライトのアンビエント色")]
		[Name(language = Language.English, value = "Ambient Color")]
		[Description(language = Language.English, value = "Ambient color of the light")]
		[Undo(Undo = false)]
		public Value.Color LightAmbientColor
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "出力時の拡大率")]
		[Description(language = Language.Japanese, value = "出力時の拡大率")]
		[Name(language = Language.English, value = "Output Magnification")]
		[Description(language = Language.English, value = "Output magnification")]
		[Undo(Undo = false)]
		public Value.Float Magnification
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "出力時の拡大率")]
		[Description(language = Language.Japanese, value = "出力時の拡大率")]
		[Name(language = Language.English, value = "Output Magnification")]
		[Description(language = Language.English, value = "Output magnification")]
		[Undo(Undo = false)]
		[Shown(Shown = false)]
		public Value.Float ExternalMagnification
		{
			get;
			private set;
		}


		[Name(language = Language.Japanese, value = "出力FPS")]
		[Description(language = Language.Japanese, value = "出力FPS")]
		[Name(language = Language.English, value = "Output FPS")]
		[Description(language = Language.English, value = "Output FPS")]
		[Undo(Undo = false)]
		public Value.Enum<FPSType> FPS
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "座標系")]
		[Description(language = Language.Japanese, value = "座標系")]
		[Name(language = Language.English, value = "Coordinate System")]
		[Description(language = Language.English, value = "Coordinate system to use")]
		[Undo(Undo = false)]
		public Value.Enum<CoordinateType> Coordinate
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "背景色")]
		[Description(language = Language.Japanese, value = "背景色")]
		[Name(language = Language.English, value = "Background Color")]
		[Description(language = Language.English, value = "Background color")]
		[Undo(Undo = false)]
		public Value.Color BackgroundColor
		{
			get;
			private set;
		}


        /// <summary>
        /// </summary>
        Value.PathForImage LasyBackgroundImage;

		[Name(language = Language.Japanese, value = "背景画像")]
		[Description(language = Language.Japanese, value = "背景画像")]
		[Name(language = Language.English, value = "Background Image")]
		[Description(language = Language.English, value = "Background image")]
		[Undo(Undo = false)]
		public Value.PathForImage BackgroundImage
		{
            get
            {
                if(LasyBackgroundImage == null)
                {
                    LasyBackgroundImage = new Value.PathForImage(Resources.GetString("ImageFilter"), false, "");
                }
                return LasyBackgroundImage;
            }
		}

		[Name(language = Language.Japanese, value = "カラースペース")]
		[Description(language = Language.Japanese, value = "カラースペース(再起動後に有効になります。)")]
		[Name(language = Language.English, value = "Color Space")]
		[Description(language = Language.English, value = "Color Space")]
		[Undo(Undo = false)]
		public Value.Enum<ColorSpaceType> ColorSpace
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "マウスの回転方向(X)")]
		[Description(language = Language.Japanese, value = "マウスの回転方向を逆にする。")]
		[Name(language = Language.English, value = "Mouse Rotation (X)")]
		[Description(language = Language.English, value = "Invert the rotation about the X-axis")]
		[Undo(Undo = false)]
		public Value.Boolean MouseRotInvX
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "マウスの回転方向(Y)")]
		[Description(language = Language.Japanese, value = "マウスの回転方向を逆にする。")]
		[Name(language = Language.English, value = "Mouse Rotation (Y)")]
		[Description(language = Language.English, value = "Invert the rotation about the Y-axis")]
		[Undo(Undo = false)]
		public Value.Boolean MouseRotInvY
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "マウスのスライド方向(X)")]
		[Description(language = Language.Japanese, value = "マウスのスライド方向を逆にする。")]
		[Name(language = Language.English, value = "Mouse Panning (X)")]
		[Description(language = Language.English, value = "Invert the pan direction about the X-axis")]
		[Undo(Undo = false)]
		public Value.Boolean MouseSlideInvX
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "マウスのスライド方向(Y)")]
		[Description(language = Language.Japanese, value = "マウスのスライド方向を逆にする。")]
		[Name(language = Language.English, value = "Mouse Panning (Y)")]
		[Description(language = Language.English, value = "Invert the pan direction about the Y-axis")]
		[Undo(Undo = false)]
		public Value.Boolean MouseSlideInvY
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "歪み方法")]
		[Description(language = Language.Japanese, value = "歪み方法")]
		[Name(language = Language.English, value = "Distortion method")]
		[Description(language = Language.English, value = "Distortion method")]
		[Undo(Undo = false)]
		public Value.Enum<DistortionMethodType> DistortionType
		{
			get;
			private set;
		}


        [Name(language = Language.Japanese, value = "言語設定")]
        [Description(language = Language.Japanese, value = "言語設定")]
        [Name(language = Language.English, value = "Language")]
        [Description(language = Language.English, value = "Langueage")]
        [Undo(Undo = false)]
        public Value.Enum<Language> GuiLanguage
        {
            get;
            private set;
        }

        [Undo(Undo = false)]
        [Shown(Shown = false)]
        public Value.Int RecordingWidth { get; private set; }

        [Undo(Undo = false)]
        [Shown(Shown = false)]
        public Value.Int RecordingHeight { get; private set; }

        [Undo(Undo = false)]
        [Shown(Shown =false)]
        [IO(Export = true, Import = true)]
        public Value.Boolean IsRecordingGuideShown { get; private set; }


        [Undo(Undo = false)]
        [Shown(Shown = false)]
        [IO(Export = true, Import = true)]
        public Value.Int RecordingStartingFrame { get; private set; }

        [Undo(Undo = false)]
        [Shown(Shown = false)]
        [IO(Export = true, Import = true)]
        public Value.Int RecordingEndingFrame { get; private set; }

        [Undo(Undo = false)]
        [Shown(Shown = false)]
        [IO(Export = true, Import = true)]
        public Value.Int RecordingFrequency { get; private set; }

        [Undo(Undo = false)]
        [Shown(Shown = false)]
        [IO(Export = true, Import = true)]
        public Value.Int RecordingHorizontalCount { get; private set; }

        [Undo(Undo = false)]
        [Shown(Shown = false)]
        [IO(Export = true, Import = true)]
        public Value.Enum<RecordingExporterType> RecordingExporter { get; private set; }

        [Undo(Undo = false)]
        [Shown(Shown = false)]
        [IO(Export = true, Import = true)]
        public Value.Enum<RecordingTransparentMethodType> RecordingTransparentMethod { get; private set; }
        public OptionValues()
		{
			RenderingMode = new Value.Enum<RenderMode>(RenderMode.Normal);
			BackgroundColor = new Value.Color(0, 0, 0, 255);
			GridColor = new Value.Color(255, 255, 255, 255);
			
			IsGridShown = new Value.Boolean(true);
			IsXYGridShown = new Value.Boolean(false);
			IsXZGridShown = new Value.Boolean(true);
			IsYZGridShown = new Value.Boolean(false);

			GridLength = new Value.Float(2, float.MaxValue, 0.1f);
			LightDirection = new Value.Vector3D(1, 1, 1, 1, -1, 1, -1, 1, -1, 0.1f, 0.1f, 0.1f);
			LightColor = new Value.Color(215, 215, 215, 255);
			LightAmbientColor = new Value.Color(40, 40, 40, 255);
			Magnification = new Value.Float(1, float.MaxValue, 0.00001f);
			ExternalMagnification = new Value.Float(1, float.MaxValue, 0.00001f);
			FPS = new Value.Enum<FPSType>(FPSType._60FPS);
			Coordinate = new Value.Enum<CoordinateType>(CoordinateType.Right);

			ColorSpace = new Value.Enum<ColorSpaceType>(ColorSpaceType.GammaSpace);

			MouseRotInvX = new Value.Boolean(false);
			MouseRotInvY = new Value.Boolean(false);
			MouseSlideInvX = new Value.Boolean(false);
			MouseSlideInvY = new Value.Boolean(false);

			DistortionType = new Value.Enum<DistortionMethodType>(DistortionMethodType.Current);

            RecordingWidth = new Value.Int(256);
            RecordingHeight = new Value.Int(256);
            IsRecordingGuideShown = new Value.Boolean(false);
            RecordingStartingFrame = new Value.Int(1);
            RecordingEndingFrame = new Value.Int(30);
            RecordingFrequency = new Value.Int(1);
            RecordingHorizontalCount = new Value.Int(4);
            RecordingExporter = new Value.Enum<RecordingExporterType>(Data.RecordingExporterType.Sprite);
            RecordingTransparentMethod = new Value.Enum<RecordingTransparentMethodType>(Data.RecordingTransparentMethodType.None);

            // Switch the language according to the OS settings
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            if (culture.Name == "ja-JP")
            {
                GuiLanguage = new Value.Enum<Language>(Language.Japanese);
            }
            else
            {
                GuiLanguage = new Value.Enum<Language>(Language.English);
            }
		}
		
		public enum RenderMode : int
		{
			[Name(value = "通常モード", language = Language.Japanese)]
			[Name(value = "Normal", language = Language.English)]
			Normal = 0,
			[Name(value = "ワイヤーフレーム", language = Language.Japanese)]
			[Name(value = "Wireframe", language = Language.English)]
			Wireframe = 1,
		}

		public enum FPSType : int
		{
			[Name(value = "60FPS", language = Language.Japanese)]
			[Name(value = "60 FPS", language = Language.English)]
			_60FPS = 1,
			[Name(value = "30FPS", language = Language.Japanese)]
			[Name(value = "30 FPS", language = Language.English)]
			_30FPS = 2,
			[Name(value = "20FPS", language = Language.Japanese)]
			[Name(value = "20 FPS", language = Language.English)]
			_20FPS = 3,
			[Name(value = "15FPS", language = Language.Japanese)]
			[Name(value = "15 FPS", language = Language.English)]
			_15FPS = 4,
		}

		public enum CoordinateType : int
		{
			[Name(value = "右手系", language = Language.Japanese)]
			[Name(value = "Right-Handed", language = Language.English)]
			Right = 0,
			[Name(value = "左手系", language = Language.Japanese)]
			[Name(value = "Left-Handed", language = Language.English)]
			Left = 1,
		}

		public enum DistortionMethodType : int
		{
			[Name(value = "現行", language = Language.Japanese)]
			[Name(value = "Current", language = Language.English)]
			Current = 0,
			[Name(value = "1.20互換", language = Language.Japanese)]
			[Name(value = "1.20 Compatible", language = Language.English)]
			Effekseer120 = 1,
			[Name(value = "無効", language = Language.Japanese)]
			[Name(value = "Disabled", language = Language.English)]
			Disabled = 2,
		}


		public enum ColorSpaceType : int
		{
			[Name(value = "ガンマスペース", language = Language.Japanese)]
			[Name(value = "GammaSpace", language = Language.English)]
			GammaSpace = 0,
			[Name(value = "リニアスペース", language = Language.Japanese)]
			[Name(value = "LinearSpace", language = Language.English)]
			LinearSpace = 1,
		}
	}
}

namespace EffekseerTool.Data
{
	public class PostEffectValues
	{
		
		[Selector(ID = 0)]
		[Name(language = Language.Japanese, value = "ブルーム")]
		[Description(language = Language.Japanese, value = "明るい光源からの光が周囲に漏れるように見えるエフェクト")]
		[Name(language = Language.English, value = "Bloom")]
		[Description(language = Language.English, value = "The effect produces fringes of light extending from the borders of bright areas in an image.")]
		[Undo(Undo = false)]
		public Value.Enum<EffectSwitch> BloomSwitch
		{
			get;
			private set;
		}

        [Selected(ID = 0, Value = 0)]
        [IO(Export = true)]
		public NoneParamater BloomNone
        {
            get;
            private set;
        }

		[Selected(ID = 0, Value = 1)]
		[IO(Export = true)]
		public BloomParamater Bloom
		{
			get;
			private set;
		}
		
		[Selector(ID = 0)]
		[Name(language = Language.Japanese, value = "トーンマッピング")]
		[Description(language = Language.Japanese, value = "")]
		[Name(language = Language.English, value = "Tone mapping")]
		[Description(language = Language.English, value = "")]
		[Undo(Undo = false)]
		public Value.Enum<TonemapAlgorithm> TonemapSelector
		{
			get;
			private set;
		}
		
        [Selected(ID = 0, Value = 0)]
        [IO(Export = true)]
		public NoneParamater TonemapNone
        {
            get;
            private set;
        }

		[Selected(ID = 0, Value = 1)]
		[IO(Export = true)]
		public TonemapReinhardParameter TonemapReinhard
		{
			get;
			private set;
		}

        public PostEffectValues()
		{
			BloomSwitch = new Value.Enum<EffectSwitch>();
			BloomNone = new NoneParamater();
			Bloom = new BloomParamater();
			TonemapSelector = new Value.Enum<TonemapAlgorithm>();
			TonemapNone = new NoneParamater();
			TonemapReinhard = new TonemapReinhardParameter();
		}

		
		public class NoneParamater
		{
			internal NoneParamater()
			{
			}
		}
		
		public enum EffectSwitch : int
		{
			[Name(value = "Off", language = Language.Japanese)]
			[Name(value = "Off", language = Language.English)]
			Off = 0,
			[Name(value = "On", language = Language.Japanese)]
			[Name(value = "On", language = Language.English)]
			On = 1,
		}
		
		public enum TonemapAlgorithm : int
		{
			[Name(value = "Off", language = Language.Japanese)]
			[Name(value = "Off", language = Language.English)]
			Off = 0,
			[Name(value = "Reinhard", language = Language.Japanese)]
			[Name(value = "Reinhard", language = Language.English)]
			Reinhard = 1,
		}

		public class BloomParamater
		{
			[Name(language = Language.Japanese, value = "明るさ")]
			[Description(language = Language.Japanese, value = "ブルームの明るさ")]
			[Name(language = Language.English, value = "Intensity")]
			[Description(language = Language.English, value = "")]
			[Undo(Undo = false)]
			public Value.Float Intensity
			{
				get;
				private set;
			}
		
			[Name(language = Language.Japanese, value = "しきい値")]
			[Description(language = Language.Japanese, value = "ブルームのしきい値")]
			[Name(language = Language.English, value = "Threshold")]
			[Description(language = Language.English, value = "")]
			[Undo(Undo = false)]
			public Value.Float Threshold
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "緩やかさ")]
			[Description(language = Language.Japanese, value = "ブルームのしきい値付近の緩やかさ")]
			[Name(language = Language.English, value = "Soft Knee")]
			[Description(language = Language.English, value = "")]
			[Undo(Undo = false)]
			public Value.Float SoftKnee
			{
				get;
				private set;
			}
			
			internal BloomParamater()
			{
				Intensity = new Value.Float(1.0f, 100.0f, 0.0f, 0.1f);
				Threshold = new Value.Float(1.0f, 100.0f, 0.0f, 0.1f);
				SoftKnee  = new Value.Float(0.5f, 1.0f, 0.0f, 0.1f);
			}
		}
		
		public class TonemapReinhardParameter
		{
			[Name(language = Language.Japanese, value = "露光")]
			[Description(language = Language.Japanese, value = "")]
			[Name(language = Language.English, value = "Exposure")]
			[Description(language = Language.English, value = "")]
			[Undo(Undo = false)]
			public Value.Float Exposure
			{
				get;
				private set;
			}
			
			internal TonemapReinhardParameter()
			{
				Exposure = new Value.Float(1.0f, 100.0f, 0.0f, 0.1f);
			}
		}
	}
}

namespace EffekseerTool.Data
{
	public class RendererCommonValues
	{
		[Name(language = Language.Japanese, value = "色/歪み画像")]
		[Description(language = Language.Japanese, value = "色/歪みを表す画像")]
		[Name(language = Language.English, value = "Texture")]
		[Description(language = Language.English, value = "Image that represents color/distortion")]
		public Value.PathForImage ColorTexture
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "ブレンド")]
		[Name(language = Language.English, value = "Blend")]
		public Value.Enum<AlphaBlendType> AlphaBlend { get; private set; }

		[Name(language = Language.Japanese, value = "フィルタ")]
		[Name(language = Language.English, value = "Filter")]
		public Value.Enum<FilterType> Filter { get; private set; }

		[Name(language = Language.Japanese, value = "外側")]
		[Name(language = Language.English, value = "Wrap")]
		public Value.Enum<WrapType> Wrap { get; private set; }

		[Name(language = Language.Japanese, value = "深度書き込み")]
		[Name(language = Language.English, value = "Depth Set")]
		public Value.Boolean ZWrite { get; private set; }

		[Name(language = Language.Japanese, value = "深度テスト")]
		[Name(language = Language.English, value = "Depth Test")]
		public Value.Boolean ZTest { get; private set; }

		[Selector(ID = 0)]
		[Name(language = Language.Japanese, value = "フェードイン")]
		[Name(language = Language.English, value = "Fade-In")]
		public Value.Enum<FadeType> FadeInType
		{
			get;
			private set;
		}

        [Selected(ID = 0, Value = 0)]
        [IO(Export = true)]
		public NoneParamater FadeInNone
        {
            get;
            private set;
        }

		[Selected(ID = 0, Value = 1)]
		[IO(Export = true)]
		public FadeInParamater FadeIn
		{
			get;
			private set;
		}

		[Selector(ID = 1)]
		[Name(language = Language.Japanese, value = "フェードアウト")]
		[Name(language = Language.English, value = "Fade-Out")]
		public Value.Enum<FadeType> FadeOutType
		{
			get;
			private set;
		}

		[Selected(ID = 1, Value = 0)]
		[IO(Export = true)]
		public NoneParamater FadeOutNone
		{
			get;
			private set;
		}

		[Selected(ID = 1, Value = 1)]
		[IO(Export = true)]
		public FadeOutParamater FadeOut
		{
			get;
			private set;
		}

		[Selector(ID = 2)]
		[Name(language = Language.Japanese, value = "UV")]
		[Name(language = Language.English, value = "UV")]
		public Value.Enum<UVType> UV
		{
			get;
			private set;
		}

		[Selected(ID = 2, Value = 0)]
		[IO(Export = true)]
		public UVDefaultParamater UVDefault { get; private set; }

		[Selected(ID = 2, Value = 1)]
		[IO(Export = true)]
		public UVFixedParamater UVFixed { get; private set; }

		[Selected(ID = 2, Value = 2)]
		[IO(Export = true)]
		public UVAnimationParamater UVAnimation { get; private set; }

		[Selected(ID = 2, Value = 3)]
		[IO(Export = true)]
		public UVScrollParamater UVScroll { get; private set; }

		[Selected(ID = 2, Value = 4)]
		[IO(Export = true)]
		public UVFCurveParamater UVFCurve { get; private set; }

		[Name(language = Language.Japanese, value = "色への影響")]
		[Description(language = Language.Japanese, value = "親ノードからの色への影響")]
		[Name(language = Language.English, value = "Inherit Color")]
		[Description(language = Language.English, value = "When this instance should copy its parent node's color")]
		public Value.Enum<ParentEffectType> ColorInheritType
		{
			get;
			private set;
		}

		[Name(language = Language.Japanese, value = "歪み")]
		[Name(language = Language.English, value = "Distortion")]
		public Value.Boolean Distortion { get; private set; }

		[Name(language = Language.Japanese, value = "歪み強度")]
		[Name(language = Language.English, value = "Distortion\nIntensity")]
		public Value.Float DistortionIntensity { get; private set; }

		internal RendererCommonValues()
		{
            ColorTexture = new Value.PathForImage(Resources.GetString("ImageFilter"), true, "");
			
			AlphaBlend = new Value.Enum<AlphaBlendType>(AlphaBlendType.Blend);
			Filter = new Value.Enum<FilterType>(FilterType.Linear);
			Wrap = new Value.Enum<WrapType>(WrapType.Repeat);

			FadeInType = new Value.Enum<FadeType>(FadeType.None);
			FadeInNone = new NoneParamater();
			FadeIn = new FadeInParamater();

			FadeOutType = new Value.Enum<FadeType>();
			FadeOutNone = new NoneParamater();
			FadeOut = new FadeOutParamater();

			UV = new Value.Enum<UVType>();

			UVDefault = new UVDefaultParamater();
			UVFixed = new UVFixedParamater();
			UVAnimation = new UVAnimationParamater();
			UVScroll = new UVScrollParamater();
			UVFCurve = new UVFCurveParamater();

			ZWrite = new Value.Boolean(false);
			ZTest = new Value.Boolean(true);

			ColorInheritType = new Value.Enum<ParentEffectType>(ParentEffectType.NotBind);

			Distortion = new Value.Boolean(false);
			DistortionIntensity = new Value.Float(1.0f, float.MaxValue, float.MinValue, 0.1f);
		}

		public class NoneParamater
		{
			internal NoneParamater()
			{
			}
		}

		public class FadeInParamater
		{
			[Name(value = "フレーム数", language = Language.Japanese)]
			[Description(language = Language.Japanese, value = "生成からフェードインが終了するまでのフレーム数")]
			[Name(value = "Frame Count", language = Language.English)]
			[Description(language = Language.English, value = "Duration in frames of the fade-in transition")]
			public Value.Float Frame { get; private set; }

			[Name(language = Language.Japanese, value = "始点速度")]
			[Description(language = Language.Japanese, value = "始点速度")]
			[Name(language = Language.English, value = "Ease In")]
			[Description(language = Language.English, value = "Initial speed (of the tween)")]
			public Value.Enum<EasingStart> StartSpeed
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "終点速度")]
			[Description(language = Language.Japanese, value = "終点速度")]
			[Name(language = Language.English, value = "Ease Out")]
			[Description(language = Language.English, value = "Final speed (of the tween)")]
			public Value.Enum<EasingEnd> EndSpeed
			{
				get;
				private set;
			}

			public FadeInParamater()
			{
				Frame = new Value.Float(1, float.MaxValue, 0);
				StartSpeed = new Value.Enum<EasingStart>(EasingStart.Start);
				EndSpeed = new Value.Enum<EasingEnd>(EasingEnd.End);
			}
		}

		public class FadeOutParamater
		{
			[Name(value = "フレーム数", language = Language.Japanese)]
			[Description(language = Language.Japanese, value = "フェードアウトが開始してから終了するまでのフレーム数")]
			[Name(value = "Frame Count", language = Language.English)]
			[Description(language = Language.English, value = "Duration in frames of the fade-out transition")]
			public Value.Float Frame { get; private set; }

			[Name(language = Language.Japanese, value = "始点速度")]
			[Description(language = Language.Japanese, value = "始点速度")]
			[Name(language = Language.English, value = "Ease In")]
			[Description(language = Language.English, value = "Initial speed (of the tween)")]
			public Value.Enum<EasingStart> StartSpeed
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "終点速度")]
			[Description(language = Language.Japanese, value = "終点速度")]
			[Name(language = Language.English, value = "Ease Out")]
			[Description(language = Language.English, value = "Final speed (of the tween)")]
			public Value.Enum<EasingEnd> EndSpeed
			{
				get;
				private set;
			}

			public FadeOutParamater()
			{
				Frame = new Value.Float(1, float.MaxValue, 0);
				StartSpeed = new Value.Enum<EasingStart>(EasingStart.Start);
				EndSpeed = new Value.Enum<EasingEnd>(EasingEnd.End);
			}
		}

		public class UVDefaultParamater
		{
		}

		public class UVFixedParamater
		{
			[Name(value = "始点", language = Language.Japanese)]
			[Name(value = "Start", language = Language.English)]
			public Value.Vector2D Start { get; private set; }
			[Name(value = "大きさ", language = Language.Japanese)]
			[Name(value = "Size", language = Language.English)]
			public Value.Vector2D Size { get; private set; }

			public UVFixedParamater()
			{
				Start = new Value.Vector2D();
				Size = new Value.Vector2D();
			}
		}

		public class UVAnimationParamater
		{
			[Name(value = "始点", language = Language.Japanese)]
			[Name(value = "Start", language = Language.English)]
			public Value.Vector2D Start { get; private set; }
			[Name(value = "大きさ", language = Language.Japanese)]
			[Name(value = "Size", language = Language.English)]
			public Value.Vector2D Size { get; private set; }

			[Name(value = "1枚あたりの時間", language = Language.Japanese)]
			[Name(value = "Frame Length", language = Language.English)]
			public Value.IntWithInifinite FrameLength { get; private set; }

			[Name(value = "横方向枚数", language = Language.Japanese)]
			[Name(value = "X-Count", language = Language.English)]
			public Value.Int FrameCountX { get; private set; }

			[Name(value = "縦方向枚数", language = Language.Japanese)]
			[Name(value = "Y-Count", language = Language.English)]
			public Value.Int FrameCountY { get; private set; }

			[Name(value = "ループ", language = Language.Japanese)]
			[Name(value = "Loop", language = Language.English)]
			public Value.Enum<LoopType> LoopType { get; private set; }

			[Name(value = "開始枚数", language = Language.Japanese)]
			[Name(value = "Start Sheet", language = Language.English)]
			public Value.IntWithRandom StartSheet { get; private set; }

			public UVAnimationParamater()
			{
				Start = new Value.Vector2D();
				Size = new Value.Vector2D();
				FrameLength = new Value.IntWithInifinite(1, false, int.MaxValue, 1);
				FrameCountX = new Value.Int(1, int.MaxValue, 1);
				FrameCountY = new Value.Int(1, int.MaxValue, 1);
				LoopType = new Value.Enum<LoopType>(RendererCommonValues.LoopType.Once);
				StartSheet = new Value.IntWithRandom(0, int.MaxValue, 0);
			}
		}

		public class UVScrollParamater
		{
			[Name(value = "始点", language = Language.Japanese)]
			[Name(value = "Start", language = Language.English)]
			public Value.Vector2DWithRandom Start { get; private set; }

			[Name(value = "大きさ", language = Language.Japanese)]
			[Name(value = "Size", language = Language.English)]
			public Value.Vector2DWithRandom Size { get; private set; }

			[Name(value = "移動速度", language = Language.Japanese)]
			[Name(value = "Scroll Speed", language = Language.English)]
			public Value.Vector2DWithRandom Speed { get; private set; }

			public UVScrollParamater()
			{
				Start = new Value.Vector2DWithRandom();
				Size = new Value.Vector2DWithRandom();
				Speed = new Value.Vector2DWithRandom();
			}
		}

		public class UVFCurveParamater
		{
			[Name(value = "始点", language = Language.Japanese)]
			[Name(value = "Start", language = Language.English)]
			[IO(Export = true)]
			public Value.FCurveVector2D Start { get; private set; }

			[Name(value = "大きさ", language = Language.Japanese)]
			[Name(value = "Size", language = Language.English)]
			[IO(Export = true)]
			public Value.FCurveVector2D Size { get; private set; }

			public UVFCurveParamater()
			{
				Start = new Value.FCurveVector2D();
				Size = new Value.FCurveVector2D();
			}
		}

		public enum FadeType : int
		{
			[Name(value = "有り", language = Language.Japanese)]
			[Name(value = "Enabled", language = Language.English)]
			Use = 1,
			[Name(value = "無し", language = Language.Japanese)]
			[Name(value = "Disabled", language = Language.English)]
			None = 0,
		}

		public enum FilterType : int
		{
			[Name(value = "最近傍", language = Language.Japanese)]
			[Name(value = "Nearest-Neighbor", language = Language.English)]
			Nearest = 0,
			[Name(value = "線形", language = Language.Japanese)]
			[Name(value = "Linear Interpolation", language = Language.English)]
			Linear = 1,
		}

		public enum WrapType : int
		{
			[Name(value = "繰り返し", language = Language.Japanese)]
			[Name(value = "Repeat", language = Language.English)]
			Repeat = 0,
			[Name(value = "クランプ", language = Language.Japanese)]
			[Name(value = "Clamp", language = Language.English)]
			Clamp = 1,
		}

		public enum UVType : int
		{
			[Name(value = "標準", language = Language.Japanese)]
			[Name(value = "Standard", language = Language.English)]
			Default = 0,
			[Name(value = "固定", language = Language.Japanese)]
			[Name(value = "Fixed", language = Language.English)]
			Fixed = 1,
			[Name(value = "アニメーション", language = Language.Japanese)]
			[Name(value = "Animation", language = Language.English)]
			Animation = 2,
			[Name(value = "スクロール", language = Language.Japanese)]
			[Name(value = "Scroll", language = Language.English)]
			Scroll = 3,
			[Name(value = "Fカーブ", language = Language.Japanese)]
			[Name(value = "F-Curve", language = Language.English)]
			FCurve = 4,
		}

		public enum LoopType : int
		{
			[Name(value = "なし", language = Language.Japanese)]
			[Name(value = "None", language = Language.English)]
			Once = 0,
			[Name(value = "ループ", language = Language.Japanese)]
			[Name(value = "Loop", language = Language.English)]
			Loop = 1,
			[Name(value = "逆ループ", language = Language.Japanese)]
			[Name(value = "Reverse Loop", language = Language.English)]
			ReverceLoop = 2,
		}
	}
}

namespace EffekseerTool.Data
{
	public class RendererValues
	{
		[Selector(ID = 0)]
		public Value.Enum<ParamaterType> Type
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 2)]
		[IO(Export = true)]
		public SpriteParamater Sprite
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 3)]
		[IO(Export = true)]
		public RibbonParamater Ribbon
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 6)]
		[IO(Export = true)]
		public TrackParameter Track
		{
			get;
			private set;
		}

        [Selected(ID = 0, Value = 4)]
        [IO(Export = true)]
        public RingParamater Ring
        {
            get;
            private set;
        }

		[Selected(ID = 0, Value = 5)]
		[IO(Export = true)]
		public ModelParamater Model
		{
			get;
			private set;
		}

		internal RendererValues()
		{
			Type = new Value.Enum<ParamaterType>(ParamaterType.Sprite);
			Sprite = new SpriteParamater();
            Ribbon = new RibbonParamater();
			Track = new TrackParameter();
            Ring = new RingParamater();
			Model = new ModelParamater();
		}

		public class SpriteParamater
		{
			[Name(language = Language.Japanese, value = "描画順")]
			[Name(language = Language.English, value = "Rendering Order")]
			public Value.Enum<RenderingOrder> RenderingOrder { get; private set; }

			[Name(language = Language.Japanese, value = "配置方法")]
			[Name(language = Language.English, value = "Configuration")]
			public Value.Enum<BillboardType> Billboard { get; private set; }

			[Name(language = Language.Japanese, value = "ブレンド")]
			[Name(language = Language.English, value = "Blend")]
			[IO(Export = false)]
			[Shown(Shown = false)]
			public Value.Enum<AlphaBlendType> AlphaBlend { get; private set; }

			[Selector(ID = 0)]
			[Name(language = Language.Japanese, value = "全体色")]
			[Name(language = Language.English, value = "Color All")]
			public Value.Enum<StandardColorType> ColorAll { get; private set; }

			[Selected(ID = 0, Value = 0)]
			public Value.Color ColorAll_Fixed { get; private set; }

			[Selected(ID = 0, Value = 1)]
			public Value.ColorWithRandom ColorAll_Random { get; private set; }

			[Selected(ID = 0, Value = 2)]
			[IO(Export = true)]
			public ColorEasingParamater ColorAll_Easing { get; private set; }

			[Selected(ID = 0, Value = 3)]
			[IO(Export = true)]
			public ColorFCurveParameter ColorAll_FCurve { get; private set; }

			[Selector(ID = 1)]
			[Name(language = Language.Japanese, value = "頂点色")]
			[Name(language = Language.English, value = "Vertex Color")]
			public Value.Enum<ColorType> Color { get; private set; }

			[Selected(ID = 1, Value = 1)]
			[Name(language = Language.Japanese, value = "左下色")]
			[Name(language = Language.English, value = "Lower-Left Color")]
			public Value.Color Color_Fixed_LL { get; private set; }

			[Selected(ID = 1, Value = 1)]
			[Name(language = Language.Japanese, value = "右下色")]
			[Name(language = Language.English, value = "Lower-Right Color")]
			public Value.Color Color_Fixed_LR { get; private set; }

			[Selected(ID = 1, Value = 1)]
			[Name(language = Language.Japanese, value = "左上色")]
			[Name(language = Language.English, value = "Upper-Left Color")]
			public Value.Color Color_Fixed_UL { get; private set; }

			[Selected(ID = 1, Value = 1)]
			[Name(language = Language.Japanese, value = "右上色")]
			[Name(language = Language.English, value = "Upper-Right Color")]
			public Value.Color Color_Fixed_UR { get; private set; }

			[Selector(ID = 2)]
			[Name(language = Language.Japanese, value = "頂点座標")]
			[Name(language = Language.English, value = "Vertex Coords")]
			public Value.Enum<PositionType> Position { get; private set; }

			[Selected(ID = 2, Value = 1)]
			[Name(language = Language.Japanese, value = "左下座標")]
			[Name(language = Language.English, value = "Lower-Left Coord")]
			public Value.Vector2D Position_Fixed_LL { get; private set; }

			[Selected(ID = 2, Value = 1)]
			[Name(language = Language.Japanese, value = "右下座標")]
			[Name(language = Language.English, value = "Lower-Right Coord")]
			public Value.Vector2D Position_Fixed_LR { get; private set; }

			[Selected(ID = 2, Value = 1)]
			[Name(language = Language.Japanese, value = "左上座標")]
			[Name(language = Language.English, value = "Upper-Left Coord")]
			public Value.Vector2D Position_Fixed_UL { get; private set; }

			[Selected(ID = 2, Value = 1)]
			[Name(language = Language.Japanese, value = "右上座標")]
			[Name(language = Language.English, value = "Upper-Right Coord")]
			public Value.Vector2D Position_Fixed_UR { get; private set; }

			[Name(language = Language.Japanese, value = "カラー画像")]
			[Description(language = Language.Japanese, value = "スプライトの色を表す画像")]
			[Name(language = Language.English, value = "Color Image")]
			[Description(language = Language.English, value = "Image representing the color of the sprite")]
			[IO(Export = false)]
			[Shown(Shown = false)]
			public Value.Path ColorTexture
			{
				get;
				private set;
			}

			public SpriteParamater()
			{
				RenderingOrder = new Value.Enum<Data.RenderingOrder>(Data.RenderingOrder.FirstCreatedInstanceIsFirst);

				Billboard = new Value.Enum<BillboardType>(BillboardType.Billboard);
				AlphaBlend = new Value.Enum<AlphaBlendType>(AlphaBlendType.Blend);

				ColorAll = new Value.Enum<StandardColorType>(StandardColorType.Fixed);
				ColorAll_Fixed = new Value.Color(255, 255, 255, 255);
				ColorAll_Random = new Value.ColorWithRandom(255, 255, 255, 255);
				ColorAll_Easing = new ColorEasingParamater();
				ColorAll_FCurve = new ColorFCurveParameter();

				Color = new Value.Enum<ColorType>(ColorType.Default);
				Color_Fixed_LL = new Value.Color(255, 255, 255, 255);
				Color_Fixed_LR = new Value.Color(255, 255, 255, 255);
				Color_Fixed_UL = new Value.Color(255, 255, 255, 255);
				Color_Fixed_UR = new Value.Color(255, 255, 255, 255);
				

				Position = new Value.Enum<PositionType>(PositionType.Default);
				Position_Fixed_LL = new Value.Vector2D(-0.5f, -0.5f);
				Position_Fixed_LR = new Value.Vector2D(0.5f, -0.5f);
				Position_Fixed_UL = new Value.Vector2D(-0.5f, 0.5f);
				Position_Fixed_UR = new Value.Vector2D(0.5f, 0.5f);
				ColorTexture = new Value.Path("画像ファイル (*.png)|*.png", true, "");
			}

            public enum ColorType : int
            {
                [Name(value = "標準", language = Language.Japanese)]
				[Name(value = "Default", language = Language.English)]
                Default = 0,
                [Name(value = "固定", language = Language.Japanese)]
				[Name(value = "Fixed", language = Language.English)]
                Fixed = 1,
            }

            public enum PositionType : int
            {
                [Name(value = "標準", language = Language.Japanese)]
				[Name(value = "Default", language = Language.English)]
                Default = 0,
                [Name(value = "固定", language = Language.Japanese)]
				[Name(value = "Fixed", language = Language.English)]
                Fixed = 1,
            }
		}

		public class RibbonParamater
		{
			[Name(language = Language.Japanese, value = "ブレンド")]
			[Name(language = Language.English, value = "Blend")]
			[IO(Export = false)]
			[Shown(Shown = false)]
			public Value.Enum<AlphaBlendType> AlphaBlend { get; private set; }

			[Name(language = Language.Japanese, value = "視点追従")]
			[Name(language = Language.English, value = "Follow Viewpoint")]
			public Value.Boolean ViewpointDependent { get; private set; }

			[Selector(ID = 0)]
			[Name(language = Language.Japanese, value = "全体色")]
			[Name(language = Language.English, value = "Color All")]
			public Value.Enum<ColorAllType> ColorAll { get; private set; }

			[Selected(ID = 0, Value = 0)]
			public Value.Color ColorAll_Fixed { get; private set; }

			[Selected(ID = 0, Value = 1)]
			public Value.ColorWithRandom ColorAll_Random { get; private set; }

			[Selected(ID = 0, Value = 2)]
			[IO(Export = true)]
			public ColorEasingParamater ColorAll_Easing { get; private set; }

			[Selector(ID = 1)]
			[Name(language = Language.Japanese, value = "頂点色")]
			[Name(language = Language.English, value = "Vertex Color")]
			public Value.Enum<ColorType> Color { get; private set; }

			[Selected(ID = 1, Value = 1)]
			[Name(language = Language.Japanese, value = "左頂点色")]
			[Name(language = Language.English, value = "Left Vertex Color")]
			public Value.Color Color_Fixed_L { get; private set; }

			[Selected(ID = 1, Value = 1)]
			[Name(language = Language.Japanese, value = "右頂点色")]
			[Name(language = Language.English, value = "Right Vertex Color")]
			public Value.Color Color_Fixed_R { get; private set; }

			[Selector(ID = 2)]
			[Name(language = Language.Japanese, value = "座標")]
			[Name(language = Language.English, value = "Position")]
			public Value.Enum<PositionType> Position { get; private set; }

			[Selected(ID = 2, Value = 1)]
			[Name(language = Language.Japanese, value = "左座標")]
			[Name(language = Language.English, value = "Left Coord")]
			public Value.Float Position_Fixed_L { get; private set; }

			[Selected(ID = 2, Value = 1)]
			[Name(language = Language.Japanese, value = "右座標")]
			[Name(language = Language.English, value = "Right Coord")]
			public Value.Float Position_Fixed_R { get; private set; }

			[Name(language = Language.Japanese, value = "スプラインの分割数")]
			[Description(language = Language.Japanese, value = "スプラインの分割数")]
			[Name(language = Language.English, value = "The number of \nspline division")]
			[Description(language = Language.English, value = "The number of spline division")]
			public Value.Int SplineDivision
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "色画像")]
			[Description(language = Language.Japanese, value = "リボンの色を表す画像")]

			[Name(language = Language.English, value = "Color Image")]
			[Description(language = Language.English, value = "Image representing the color of the ribbon")]
			[IO(Export = false)]
			[Shown(Shown = false)]
			public Value.Path ColorTexture
			{
				get;
				private set;
			}

			public RibbonParamater()
			{
				AlphaBlend = new Value.Enum<AlphaBlendType>(AlphaBlendType.Blend);
				ViewpointDependent = new Value.Boolean(false);
				ColorAll = new Value.Enum<ColorAllType>(ColorAllType.Fixed);
				ColorAll_Fixed = new Value.Color(255, 255, 255, 255);
				ColorAll_Random = new Value.ColorWithRandom(255, 255, 255, 255);
				ColorAll_Easing = new ColorEasingParamater();

				Color = new Value.Enum<ColorType>(ColorType.Default);
				Color_Fixed_L = new Value.Color(255, 255, 255, 255);
				Color_Fixed_R = new Value.Color(255, 255, 255, 255);

				Position = new Value.Enum<PositionType>(PositionType.Default);
				Position_Fixed_L = new Value.Float(-0.5f);
				Position_Fixed_R = new Value.Float(0.5f);

				SplineDivision = new Value.Int(1, int.MaxValue, 1);

				ColorTexture = new Value.Path(Resources.GetString("ImageFilter"), true, "");
			}

            public enum ColorAllType : int
            {
                [Name(value = "固定", language = Language.Japanese)]
				[Name(value = "Fixed", language = Language.English)]
                Fixed = 0,
                [Name(value = "ランダム", language = Language.Japanese)]
				[Name(value = "Random", language = Language.English)]
                Random = 1,
                [Name(value = "イージング", language = Language.Japanese)]
				[Name(value = "Easing", language = Language.English)]
                Easing = 2,
            }

            public enum ColorType : int
            {
                [Name(value = "標準", language = Language.Japanese)]
				[Name(value = "Default", language = Language.English)]
                Default = 0,
                [Name(value = "固定", language = Language.Japanese)]
				[Name(value = "Fixed", language = Language.English)]
                Fixed = 1,
            }

            public enum PositionType : int
            {
                [Name(value = "標準", language = Language.Japanese)]
				[Name(value = "Default", language = Language.English)]
                Default = 0,
                [Name(value = "固定", language = Language.Japanese)]
				[Name(value = "Fixed", language = Language.English)]
                Fixed = 1,
            }
		}

        public class RingParamater
        {
            [Name(language = Language.Japanese, value = "描画順")]
			[Name(language = Language.English, value = "Rendering Order")]
            public Value.Enum<RenderingOrder> RenderingOrder { get; private set; }

            [Name(language = Language.Japanese, value = "配置方法")]
			[Name(language = Language.English, value = "Configuration")]
            public Value.Enum<BillboardType> Billboard { get; private set; }

            [Name(language = Language.Japanese, value = "ブレンド")]
			[Name(language = Language.English, value = "Blend")]
			[IO(Export = false)]
			[Shown(Shown = false)]
            public Value.Enum<AlphaBlendType> AlphaBlend { get; private set; }

            [Name(language = Language.Japanese, value = "頂点数")]
			[Name(language = Language.English, value = "Vertex Count")]
            public Value.Int VertexCount { get; private set; }

            [Selector(ID = 0)]
            [Name(language = Language.Japanese, value = "表示角度")]
			[Name(language = Language.English, value = "Viewing Angle")]
            public Value.Enum<ViewingAngleType> ViewingAngle { get; private set; }

            [Selected(ID = 0, Value = 0)]
            public Value.Float ViewingAngle_Fixed { get; private set; }

            [Selected(ID = 0, Value = 1)]
            public Value.FloatWithRandom ViewingAngle_Random { get; private set; }

            [Selected(ID = 0, Value = 2)]
            [IO(Export = true)]
            public FloatEasingParamater ViewingAngle_Easing { get; private set; }

            [Selector(ID = 1)]
            [Name(language = Language.Japanese, value = "外輪")]
			[Name(language = Language.English, value = "Outer")]
            public Value.Enum<LocationType> Outer { get; private set; }

            [Selected(ID = 1, Value = 0)]
			[IO(Export = true)]
            public FixedLocation Outer_Fixed { get; private set; }

            [Selected(ID = 1, Value = 1)]
			[IO(Export = true)]
            public PVALocation Outer_PVA { get; private set; }

            [Selected(ID = 1, Value = 2)]
            [IO(Export = true)]
            public Vector2DEasingParamater Outer_Easing { get; private set; }

            [Selector(ID = 2)]
            [Name(language = Language.Japanese, value = "内輪")]
			[Name(language = Language.English, value = "Inner")]
            public Value.Enum<LocationType> Inner { get; private set; }

            [Selected(ID = 2, Value = 0)]
			[IO(Export = true)]
            public FixedLocation Inner_Fixed { get; private set; }

            [Selected(ID = 2, Value = 1)]
			[IO(Export = true)]
            public PVALocation Inner_PVA { get; private set; }

            [Selected(ID = 2, Value = 2)]
            [IO(Export = true)]
            public Vector2DEasingParamater Inner_Easing { get; private set; }

            [Selector(ID = 3)]
            [Name(language = Language.Japanese, value = "中心比率")]
			[Name(language = Language.English, value = "Center Ratio")]
            public Value.Enum<CenterRatioType> CenterRatio { get; private set; }

            [Selected(ID = 3, Value = 0)]
            public Value.Float CenterRatio_Fixed { get; private set; }

            [Selected(ID = 3, Value = 1)]
            public Value.FloatWithRandom CenterRatio_Random { get; private set; }

            [Selected(ID = 3, Value = 2)]
            [IO(Export = true)]
            public FloatEasingParamater CenterRatio_Easing { get; private set; }

            [Selector(ID = 4)]
            [Name(language = Language.Japanese, value = "外輪色")]
			[Name(language = Language.English, value = "Outer Color")]
            public Value.Enum<ColorType> OuterColor { get; private set; }

            [Selected(ID = 4, Value = 0)]
            public Value.Color OuterColor_Fixed { get; private set; }

            [Selected(ID = 4, Value = 1)]
            public Value.ColorWithRandom OuterColor_Random { get; private set; }

            [Selected(ID = 4, Value = 2)]
            [IO(Export = true)]
            public ColorEasingParamater OuterColor_Easing { get; private set; }

            [Selector(ID = 5)]
            [Name(language = Language.Japanese, value = "中心色")]
			[Name(language = Language.English, value = "Center Color")]
            public Value.Enum<ColorType> CenterColor { get; private set; }

            [Selected(ID = 5, Value = 0)]
			[IO(Export = true)]
            public Value.Color CenterColor_Fixed { get; private set; }

            [Selected(ID = 5, Value = 1)]
			[IO(Export = true)]
            public Value.ColorWithRandom CenterColor_Random { get; private set; }

            [Selected(ID = 5, Value = 2)]
            [IO(Export = true)]
            public ColorEasingParamater CenterColor_Easing { get; private set; }

            [Selector(ID = 6)]
            [Name(language = Language.Japanese, value = "内輪色")]
			[Name(language = Language.English, value = "Inner Color")]
            public Value.Enum<ColorType> InnerColor { get; private set; }

            [Selected(ID = 6, Value = 0)]
            public Value.Color InnerColor_Fixed { get; private set; }

            [Selected(ID = 6, Value = 1)]
            public Value.ColorWithRandom InnerColor_Random { get; private set; }

            [Selected(ID = 6, Value = 2)]
            [IO(Export = true)]
            public ColorEasingParamater InnerColor_Easing { get; private set; }


            [Name(language = Language.Japanese, value = "色画像")]
            [Description(language = Language.Japanese, value = "リボンの色を表す画像")]
			[Name(language = Language.English, value = "Color Image")]
			[Description(language = Language.English, value = "Image representing the color of the ribbon")]
			[IO(Export = false)]
			[Shown(Shown = false)]
            public Value.Path ColorTexture
            {
                get;
                private set;
            }

            public RingParamater()
            {
                RenderingOrder = new Value.Enum<Data.RenderingOrder>(Data.RenderingOrder.FirstCreatedInstanceIsFirst);

                Billboard = new Value.Enum<BillboardType>(BillboardType.Fixed);
                AlphaBlend = new Value.Enum<AlphaBlendType>(AlphaBlendType.Blend);

                VertexCount = new Value.Int(16, 256, 3);

                ViewingAngle = new Value.Enum<ViewingAngleType>(ViewingAngleType.Fixed);
                ViewingAngle_Fixed = new Value.Float(360.0f, 360.0f, 0.0f);
                ViewingAngle_Random = new Value.FloatWithRandom(360.0f, 360.0f, 0.0f);
                ViewingAngle_Easing = new FloatEasingParamater(360.0f, 360.0f, 0.0f);

                Outer = new Value.Enum<LocationType>(LocationType.Fixed);
                Outer_Fixed = new FixedLocation(2.0f, 0.0f);
                Outer_PVA = new PVALocation(2.0f, 0.0f);
                Outer_Easing = new Vector2DEasingParamater();

                Inner = new Value.Enum<LocationType>(LocationType.Fixed);
                Inner_Fixed = new FixedLocation(1.0f, 0.0f);
                Inner_PVA = new PVALocation(1.0f, 0.0f);
                Inner_Easing = new Vector2DEasingParamater();

                CenterRatio = new Value.Enum<CenterRatioType>(CenterRatioType.Fixed);
                CenterRatio_Fixed = new Value.Float(0.5f, 1.0f, 0.0f);
                CenterRatio_Random = new Value.FloatWithRandom(0.5f, 1.0f, 0.0f);
                CenterRatio_Easing = new FloatEasingParamater(0.5f, 1.0f, 0.0f);

                OuterColor = new Value.Enum<ColorType>(ColorType.Fixed);
                OuterColor_Fixed = new Value.Color(255, 255, 255, 0);
                OuterColor_Random = new Value.ColorWithRandom(255, 255, 255, 0);
                OuterColor_Easing = new ColorEasingParamater();

                CenterColor = new Value.Enum<ColorType>(ColorType.Fixed);
                CenterColor_Fixed = new Value.Color(255, 255, 255, 255);
                CenterColor_Random = new Value.ColorWithRandom(255, 255, 255, 255);
                CenterColor_Easing = new ColorEasingParamater();

                InnerColor = new Value.Enum<ColorType>(ColorType.Fixed);
                InnerColor_Fixed = new Value.Color(255, 255, 255, 0);
                InnerColor_Random = new Value.ColorWithRandom(255, 255, 255, 0);
                InnerColor_Easing = new ColorEasingParamater();

                ColorTexture = new Value.Path(Resources.GetString("ImageFilter"), true, "");
            }

            public enum ViewingAngleType : int
            {
                [Name(value = "位置", language = Language.Japanese)]
				[Name(value = "Fixed", language = Language.English)]
                Fixed = 0,
                [Name(value = "ランダム", language = Language.Japanese)]
				[Name(value = "Random", language = Language.English)]
                Random = 1,
                [Name(value = "イージング", language = Language.Japanese)]
				[Name(value = "Easing", language = Language.English)]
                Easing = 2,
            }

            public enum LocationType : int
            {
                [Name(value = "位置", language = Language.Japanese)]
				[Name(value = "Fixed", language = Language.English)]
                Fixed = 0,
                [Name(value = "位置・速度・加速度", language = Language.Japanese)]
				[Name(value = "PVA", language = Language.English)]
                PVA = 1,
                [Name(value = "イージング", language = Language.Japanese)]
				[Name(value = "Easing", language = Language.English)]
                Easing = 2,
            }

            public class FixedLocation
            {
                [Name(language = Language.Japanese, value = "位置")]
                [Description(language = Language.Japanese, value = "インスタンスの位置")]
				[Name(language = Language.English, value = "Position")]
				[Description(language = Language.English, value = "Position of the instance")]
                public Value.Vector2D Location
                {
                    get;
                    private set;
                }

                internal FixedLocation(float x = 0.0f, float y = 0.0f)
                {
                    Location = new Value.Vector2D(x, y);
                }
            }

            public enum CenterRatioType : int
            {
                [Name(value = "位置", language = Language.Japanese)]
				[Name(value = "Fixed", language = Language.English)]
                Fixed = 0,
                [Name(value = "ランダム", language = Language.Japanese)]
				[Name(value = "Random", language = Language.English)]
                Random = 1,
                [Name(value = "イージング", language = Language.Japanese)]
				[Name(value = "Easing", language = Language.English)]
                Easing = 2,
            }

            public class PVALocation
            {
                [Name(language = Language.Japanese, value = "位置")]
                [Description(language = Language.Japanese, value = "インスタンスの初期位置")]
				[Name(language = Language.English, value = "Pos")]
				[Description(language = Language.English, value = "Position of the instance")]
                public Value.Vector2DWithRandom Location
                {
                    get;
                    private set;
                }

                [Name(language = Language.Japanese, value = "速度")]
                [Description(language = Language.Japanese, value = "インスタンスの初期速度")]
				[Name(language = Language.English, value = "Speed")]
				[Description(language = Language.English, value = "Initial velocity of the instance")]
                public Value.Vector2DWithRandom Velocity
                {
                    get;
                    private set;
                }

                [Name(language = Language.Japanese, value = "加速度")]
                [Description(language = Language.Japanese, value = "インスタンスの初期加速度")]
				[Name(language = Language.English, value = "Accel")]
				[Description(language = Language.English, value = "Acceleration of the instance")]
                public Value.Vector2DWithRandom Acceleration
                {
                    get;
                    private set;
                }

                internal PVALocation(float x = 0.0f, float y = 0.0f)
                {
                    Location = new Value.Vector2DWithRandom(x, y);
                    Velocity = new Value.Vector2DWithRandom(0, 0);
                    Acceleration = new Value.Vector2DWithRandom(0, 0);
                }
            }

            public enum ColorType : int
            {
                [Name(value = "固定", language = Language.Japanese)]
				[Name(value = "Fixed", language = Language.English)]
                Fixed = 0,
                [Name(value = "ランダム", language = Language.Japanese)]
				[Name(value = "Random", language = Language.English)]
                Random = 1,
                [Name(value = "イージング", language = Language.Japanese)]
				[Name(value = "Easing", language = Language.English)]
                Easing = 2,
            }
        }

		public class ModelParamater
		{
			[Name(language = Language.Japanese, value = "モデル")]
			[Description(language = Language.Japanese, value = "モデルファイル")]
			[Name(language = Language.English, value = "Model")]
			[Description(language = Language.English, value = "Model File")]
			public Value.PathForModel Model
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "法線画像")]
			[Description(language = Language.Japanese, value = "法線を表す画像")]
			[Name(language = Language.English, value = "Normal Map")]
			[Description(language = Language.English, value = "Image representing normal vectors")]
			public Value.PathForImage NormalTexture
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "配置方法")]
			[Name(language = Language.English, value = "Configuration")]
			public Value.Enum<BillboardType> Billboard { get; private set; }

			[Name(language = Language.Japanese, value = "ライティング")]
			[Name(language = Language.English, value = "Lighting")]
			public Value.Boolean Lighting { get; private set; }

			[Name(language = Language.Japanese, value = "カリング")]
			[Name(language = Language.English, value = "Culling")]
			public Value.Enum<CullingValues> Culling { get; private set; }

			public ModelParamater()
			{
                Model = new Value.PathForModel(Resources.GetString("ModelFilter"), true, "");
                NormalTexture = new Value.PathForImage(Resources.GetString("ImageFilter"), true, "");

				Billboard = new Value.Enum<BillboardType>(BillboardType.Fixed);

				Lighting = new Value.Boolean(true);
				Culling = new Value.Enum<CullingValues>(Data.CullingValues.Front);

				Color = new Value.Enum<StandardColorType>(StandardColorType.Fixed);
				Color_Fixed = new Value.Color(255, 255, 255, 255);
				Color_Random = new Value.ColorWithRandom(255, 255, 255, 255);
				Color_Easing = new ColorEasingParamater();
				Color_FCurve = new ColorFCurveParameter();
			}

			[Selector(ID = 0)]
			[Name(language = Language.Japanese, value = "全体色")]
			public Value.Enum<StandardColorType> Color { get; private set; }

			[Selected(ID = 0, Value = 0)]
			public Value.Color Color_Fixed { get; private set; }

			[Selected(ID = 0, Value = 1)]
			public Value.ColorWithRandom Color_Random { get; private set; }

			[Selected(ID = 0, Value = 2)]
			[IO(Export = true)]
			public ColorEasingParamater Color_Easing { get; private set; }

			[Selected(ID = 0, Value = 3)]
			[IO(Export = true)]
			public ColorFCurveParameter Color_FCurve { get; private set; }
		}

		public class TrackParameter
		{
			[Selector(ID = 10)]
			[Name(language = Language.Japanese, value = "幅前方")]
			[Name(language = Language.English, value = "Front Size")]
			public Value.Enum<TrackSizeType> TrackSizeFor { get; private set; }

			[Selected(ID = 10, Value = 0)]
			public Value.Float TrackSizeFor_Fixed { get; private set; }

			[Selector(ID = 11)]
			[Name(language = Language.Japanese, value = "幅中間")]
			[Name(language = Language.English, value = "Middle Size")]
			public Value.Enum<TrackSizeType> TrackSizeMiddle { get; private set; }

			[Selected(ID = 11, Value = 0)]
			public Value.Float TrackSizeMiddle_Fixed { get; private set; }

			[Selector(ID = 12)]
			[Name(language = Language.Japanese, value = "幅後方")]
			[Name(language = Language.English, value = "Back Size")]
			public Value.Enum<TrackSizeType> TrackSizeBack { get; private set; }

			[Selected(ID = 12, Value = 0)]
			public Value.Float TrackSizeBack_Fixed { get; private set; }

			[Name(language = Language.Japanese, value = "スプラインの分割数")]
			[Description(language = Language.Japanese, value = "スプラインの分割数")]
			[Name(language = Language.English, value = "The number of \nspline division")]
			[Description(language = Language.English, value = "The number of spline division")]
			public Value.Int SplineDivision
			{
				get;
				private set;
			}

			[Selector(ID = 1)]
			[Name(language = Language.Japanese, value = "色・左")]
			[Name(language = Language.English, value = "Color, Left")]
			public Value.Enum<StandardColorType> ColorLeft { get; private set; }

			[Selected(ID = 1, Value = 0)]
			public Value.Color ColorLeft_Fixed { get; private set; }

			[Selected(ID = 1, Value = 1)]
			public Value.ColorWithRandom ColorLeft_Random { get; private set; }

			[Selected(ID = 1, Value = 2)]
			[IO(Export = true)]
			public ColorEasingParamater ColorLeft_Easing { get; private set; }

			[Selected(ID = 1, Value = 3)]
			[IO(Export = true)]
			public ColorFCurveParameter ColorLeft_FCurve { get; private set; }

			[Selector(ID = 2)]
			[Name(language = Language.Japanese, value = "色・左中間")]
			[Name(language = Language.English, value = "Color, Left-Center")]
			public Value.Enum<StandardColorType> ColorLeftMiddle { get; private set; }

			[Selected(ID = 2, Value = 0)]
			public Value.Color ColorLeftMiddle_Fixed { get; private set; }

			[Selected(ID = 2, Value = 1)]
			public Value.ColorWithRandom ColorLeftMiddle_Random { get; private set; }

			[Selected(ID = 2, Value = 2)]
			[IO(Export = true)]
			public ColorEasingParamater ColorLeftMiddle_Easing { get; private set; }

			[Selected(ID = 2, Value = 3)]
			[IO(Export = true)]
			public ColorFCurveParameter ColorLeftMiddle_FCurve { get; private set; }

			[Selector(ID = 3)]
			[Name(language = Language.Japanese, value = "色・中央")]
			[Name(language = Language.English, value = "Color, Center")]
			public Value.Enum<StandardColorType> ColorCenter { get; private set; }

			[Selected(ID = 3, Value = 0)]
			public Value.Color ColorCenter_Fixed { get; private set; }

			[Selected(ID = 3, Value = 1)]
			public Value.ColorWithRandom ColorCenter_Random { get; private set; }

			[Selected(ID = 3, Value = 2)]
			[IO(Export = true)]
			public ColorEasingParamater ColorCenter_Easing { get; private set; }

			[Selected(ID = 3, Value = 3)]
			[IO(Export = true)]
			public ColorFCurveParameter ColorCenter_FCurve { get; private set; }

			[Selector(ID = 4)]
			[Name(language = Language.Japanese, value = "色・中央中間")]
			[Name(language = Language.English, value = "Color, Center-Mid.")]
			public Value.Enum<StandardColorType> ColorCenterMiddle { get; private set; }

			[Selected(ID = 4, Value = 0)]
			public Value.Color ColorCenterMiddle_Fixed { get; private set; }

			[Selected(ID = 4, Value = 1)]
			public Value.ColorWithRandom ColorCenterMiddle_Random { get; private set; }

			[Selected(ID = 4, Value = 2)]
			[IO(Export = true)]
			public ColorEasingParamater ColorCenterMiddle_Easing { get; private set; }

			[Selected(ID = 4, Value = 3)]
			[IO(Export = true)]
			public ColorFCurveParameter ColorCenterMiddle_FCurve { get; private set; }

			[Selector(ID = 5)]
			[Name(language = Language.Japanese, value = "色・右")]
			[Name(language = Language.English, value = "Color, Right")]
			public Value.Enum<StandardColorType> ColorRight { get; private set; }

			[Selected(ID = 5, Value = 0)]
			public Value.Color ColorRight_Fixed { get; private set; }

			[Selected(ID = 5, Value = 1)]
			public Value.ColorWithRandom ColorRight_Random { get; private set; }

			[Selected(ID = 5, Value = 2)]
			[IO(Export = true)]
			public ColorEasingParamater ColorRight_Easing { get; private set; }

			[Selected(ID = 5, Value = 3)]
			[IO(Export = true)]
			public ColorFCurveParameter ColorRight_FCurve { get; private set; }

			[Selector(ID = 6)]
			[Name(language = Language.Japanese, value = "色・右中間")]
			[Name(language = Language.English, value = "Color, Right-Center")]
			public Value.Enum<StandardColorType> ColorRightMiddle { get; private set; }

			[Selected(ID = 6, Value = 0)]
			public Value.Color ColorRightMiddle_Fixed { get; private set; }

			[Selected(ID = 6, Value = 1)]
			public Value.ColorWithRandom ColorRightMiddle_Random { get; private set; }

			[Selected(ID = 6, Value = 2)]
			[IO(Export = true)]
			public ColorEasingParamater ColorRightMiddle_Easing { get; private set; }

			[Selected(ID = 6, Value = 3)]
			[IO(Export = true)]
			public ColorFCurveParameter ColorRightMiddle_FCurve { get; private set; }

			
			public TrackParameter()
			{
				TrackSizeFor = new Value.Enum<TrackSizeType>(TrackSizeType.Fixed);
				TrackSizeFor_Fixed = new Value.Float(1, float.MaxValue, 0);

				TrackSizeMiddle = new Value.Enum<TrackSizeType>(TrackSizeType.Fixed);
				TrackSizeMiddle_Fixed = new Value.Float(1, float.MaxValue, 0);

				TrackSizeBack = new Value.Enum<TrackSizeType>(TrackSizeType.Fixed);
				TrackSizeBack_Fixed = new Value.Float(1, float.MaxValue, 0);

				ColorLeft = new Value.Enum<StandardColorType>(StandardColorType.Fixed);
				ColorLeft_Fixed = new Value.Color(255, 255, 255, 255);
				ColorLeft_Random = new Value.ColorWithRandom(255, 255, 255, 255);
				ColorLeft_Easing = new ColorEasingParamater();
				ColorLeft_FCurve = new ColorFCurveParameter();

				ColorLeftMiddle = new Value.Enum<StandardColorType>(StandardColorType.Fixed);
				ColorLeftMiddle_Fixed = new Value.Color(255, 255, 255, 255);
				ColorLeftMiddle_Random = new Value.ColorWithRandom(255, 255, 255, 255);
				ColorLeftMiddle_Easing = new ColorEasingParamater();
				ColorLeftMiddle_FCurve = new ColorFCurveParameter();

				ColorCenter = new Value.Enum<StandardColorType>(StandardColorType.Fixed);
				ColorCenter_Fixed = new Value.Color(255, 255, 255, 255);
				ColorCenter_Random = new Value.ColorWithRandom(255, 255, 255, 255);
				ColorCenter_Easing = new ColorEasingParamater();
				ColorCenter_FCurve = new ColorFCurveParameter();

				ColorCenterMiddle = new Value.Enum<StandardColorType>(StandardColorType.Fixed);
				ColorCenterMiddle_Fixed = new Value.Color(255, 255, 255, 255);
				ColorCenterMiddle_Random = new Value.ColorWithRandom(255, 255, 255, 255);
				ColorCenterMiddle_Easing = new ColorEasingParamater();
				ColorCenterMiddle_FCurve = new ColorFCurveParameter();

				ColorRight = new Value.Enum<StandardColorType>(StandardColorType.Fixed);
				ColorRight_Fixed = new Value.Color(255, 255, 255, 255);
				ColorRight_Random = new Value.ColorWithRandom(255, 255, 255, 255);
				ColorRight_Easing = new ColorEasingParamater();
				ColorRight_FCurve = new ColorFCurveParameter();

				ColorRightMiddle = new Value.Enum<StandardColorType>(StandardColorType.Fixed);
				ColorRightMiddle_Fixed = new Value.Color(255, 255, 255, 255);
				ColorRightMiddle_Random = new Value.ColorWithRandom(255, 255, 255, 255);
				ColorRightMiddle_Easing = new ColorEasingParamater();
				ColorRightMiddle_FCurve = new ColorFCurveParameter();

				SplineDivision = new Value.Int(1, int.MaxValue, 1);
			}
		}

		public enum BillboardType : int
		{
			[Name(value = "ビルボード", language = Language.Japanese)]
			[Name(value = "Billboard", language = Language.English)]
			Billboard = 0,
			[Name(value = "Z軸回転ビルボード", language = Language.Japanese)]
			[Name(value = "Rotated Billboard", language = Language.English)]
			RotatedBillboard = 3,
			[Name(value = "Y軸固定", language = Language.Japanese)]
			[Name(value = "Fixed Y-Axis", language = Language.English)]
			YAxisFixed = 1,
			[Name(value = "固定", language = Language.Japanese)]
			[Name(value = "Fixed", language = Language.English)]
			Fixed = 2,
		}

		public enum ParamaterType : int
		{
			[Name(value = "無し", language = Language.Japanese)]
			[Name(value = "None", language = Language.English)]
			[Icon(resourceName = "NodeEmpty")]
			None = 0,
			//Particle = 1,
			[Name(value = "スプライト", language = Language.Japanese)]
			[Name(value = "Sprite", language = Language.English)]
			[Icon(resourceName = "NodeSprite")]
			Sprite = 2,
			[Name(value = "リボン", language = Language.Japanese)]
			[Name(value = "Ribbon", language = Language.English)]
			[Icon(resourceName = "NodeRibbon")]
            Ribbon = 3,
			[Name(value = "軌跡", language = Language.Japanese)]
			[Name(value = "Track", language = Language.English)]
			[Icon(resourceName = "NodeTrack")]
			Track = 6,
            [Name(value = "リング", language = Language.Japanese)]
			[Name(value = "Ring", language = Language.English)]
			[Icon(resourceName = "NodeRing")]
            Ring = 4,
			[Name(value = "モデル", language = Language.Japanese)]
			[Name(value = "Model", language = Language.English)]
			[Icon(resourceName = "NodeModel")]
			Model = 5,
		}

	}
}

namespace EffekseerTool.Data
{
	public class RotationValues
	{
		[Selector(ID = 0)]
		public Value.Enum<ParamaterType> Type
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 0)]
		[IO(Export = true)]
		public FixedParamater Fixed
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 1)]
		[IO(Export = true)]
		public PVAParamater PVA
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 2)]
		[IO(Export = true)]
		public Vector3DEasingParamater Easing
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 3)]
		[IO(Export = true)]
		public AxisPVAParamater AxisPVA
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 4)]
		[IO(Export = true)]
		public AxisEasingParamater AxisEasing
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 5)]
		[IO(Export = true)]
		public RotationFCurveParamater RotationFCurve
		{
			get;
			private set;
		}

		internal RotationValues()
		{
			Type = new Value.Enum<ParamaterType>(ParamaterType.Fixed);
			Fixed = new FixedParamater();
			PVA = new PVAParamater();
			Easing = new Vector3DEasingParamater();
			AxisPVA = new AxisPVAParamater();
			AxisEasing = new AxisEasingParamater();
			RotationFCurve = new RotationFCurveParamater();
		}

		public class FixedParamater
		{
			[Name(language = Language.Japanese, value = "角度(度)")]
			[Description(language = Language.Japanese, value = "インスタンスの角度")]
			[Name(language = Language.English, value = "Angle")]
			[Description(language = Language.English, value = "Rotation of the instance, in degrees")]
			public Value.Vector3D Rotation
			{
				get;
				private set;
			}

			internal FixedParamater()
			{
				Rotation = new Value.Vector3D(0, 0, 0);
			}
		}

		public class PVAParamater
		{
			[Name(language = Language.Japanese, value = "角度(度)")]
			[Description(language = Language.Japanese, value = "インスタンスの初期角度")]
			[Name(language = Language.English, value = "Angle")]
			[Description(language = Language.English, value = "Initial rotation of the instance, in degrees")]
			public Value.Vector3DWithRandom Rotation
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "角速度(度)")]
			[Description(language = Language.Japanese, value = "インスタンスの角速度")]
			[Name(language = Language.English, value = "Angular\nVelocity")]
			[Description(language = Language.English, value = "Initial angular velocity of the instance, in degrees")]
			public Value.Vector3DWithRandom Velocity
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "角加速度(度)")]
			[Description(language = Language.Japanese, value = "インスタンスの初期角加速度")]
			[Name(language = Language.English, value = "Angular\nAccel")]
			[Description(language = Language.English, value = "Acceleration of the instance's angular velocity, in degrees")]
			public Value.Vector3DWithRandom Acceleration
			{
				get;
				private set;
			}

			internal PVAParamater()
			{
				Rotation = new Value.Vector3DWithRandom(0, 0, 0);
				Velocity = new Value.Vector3DWithRandom(0, 0, 0);
				Acceleration = new Value.Vector3DWithRandom(0, 0, 0);
			}
		}

		public class AxisPVAParamater
		{
			[Name(language = Language.Japanese, value = "回転軸")]
			[Description(language = Language.Japanese, value = "インスタンスの回転軸")]
			[Name(language = Language.English, value = "Axis of\nRotation")]
			[Description(language = Language.English, value = "Axis of rotation of the instance")]
			public Value.Vector3DWithRandom Axis
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "角度(度)")]
			[Description(language = Language.Japanese, value = "インスタンスの初期角度")]
			[Name(language = Language.English, value = "Angle")]
			[Description(language = Language.English, value = "Initial rotation of the instance, in degrees")]
			public Value.FloatWithRandom Rotation
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "角速度(度)")]
			[Description(language = Language.Japanese, value = "インスタンスの角速度")]
			[Name(language = Language.English, value = "Angular\nVelocity")]
			[Description(language = Language.English, value = "Initial angular velocity of the instance, in degrees")]
			public Value.FloatWithRandom Velocity
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "角加速度(度)")]
			[Description(language = Language.Japanese, value = "インスタンスの初期角加速度")]
			[Name(language = Language.English, value = "Angular\nAccel")]
			[Description(language = Language.English, value = "Acceleration of the instance's angular velocity, in degrees")]
			public Value.FloatWithRandom Acceleration
			{
				get;
				private set;
			}

			internal AxisPVAParamater()
			{
				Axis = new Value.Vector3DWithRandom(0, 1, 0);
				Rotation = new Value.FloatWithRandom(0);
				Velocity = new Value.FloatWithRandom(0);
				Acceleration = new Value.FloatWithRandom(0);
			}
		}
		

		public class AxisEasingParamater
		{
			[Name(language = Language.Japanese, value = "回転軸")]
			[Description(language = Language.Japanese, value = "インスタンスの回転軸")]
			[Name(language = Language.English, value = "Axis of\nRotation")]
			[Description(language = Language.English, value = "Axis of rotation of the instance")]
			public Value.Vector3DWithRandom Axis
			{
				get;
				private set;
			}

			[IO(Export = true)]
			public FloatEasingParamater Easing
			{
				get;
				private set;
			}

			internal AxisEasingParamater()
			{
				Axis = new Value.Vector3DWithRandom(0, 1, 0);
				Easing = new FloatEasingParamater();
			}
		}

		public class RotationFCurveParamater
		{
			[Name(language = Language.Japanese, value = "Fカーブ")]
			[Description(language = Language.Japanese, value = "Fカーブ")]
			[Name(language = Language.English, value = "F-Curve")]
			[Description(language = Language.English, value = "F-Curve")]
			[Shown(Shown = true)]
			public Value.FCurveVector3D FCurve
			{
				get;
				private set;
			}

			public RotationFCurveParamater()
			{
				FCurve = new Value.FCurveVector3D();

				FCurve.X.DefaultValueRangeMax = 180.0f;
				FCurve.X.DefaultValueRangeMin = -180.0f;
				FCurve.Y.DefaultValueRangeMax = 180.0f;
				FCurve.Y.DefaultValueRangeMin = -180.0f;
				FCurve.Z.DefaultValueRangeMax = 180.0f;
				FCurve.Z.DefaultValueRangeMin = -180.0f;
			}
		}

		public enum ParamaterType : int
		{
			[Name(value = "角度", language = Language.Japanese)]
			[Name(value = "Fixed Angle", language = Language.English)]
			Fixed = 0,
			[Name(value = "角度・速度・加速度", language = Language.Japanese)]
			[Name(value = "PVA", language = Language.English)]
			PVA = 1,
			[Name(value = "イージング", language = Language.Japanese)]
			[Name(value = "Easing", language = Language.English)]
			Easing = 2,
			[Name(value = "任意軸 角度・速度・加速度", language = Language.Japanese)]
			[Name(value = "PVA (Arbitrary Axis)", language = Language.English)]
			AxisPVA = 3,
			[Name(value = "任意軸 イージング", language = Language.Japanese)]
			[Name(value = "Easing (Arbitrary Axis)", language = Language.English)]
			AxisEasing = 4,
			[Name(value = "角度(Fカーブ)", language = Language.Japanese)]
			[Name(value = "Rotation (F-Curve)", language = Language.English)]
			RotationFCurve = 5,
		}
	}
}

namespace EffekseerTool.Data
{
	public class ScaleValues
	{
		[Selector(ID = 0)]
		public Value.Enum<ParamaterType> Type
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 0)]
		[IO(Export = true)]
		public FixedParamater Fixed
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 1)]
		[IO(Export = true)]
		public PVAParamater PVA
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 2)]
		[IO(Export = true)]
		public Vector3DEasingParamater Easing
		{
			get;
			private set;
		}
		
		[Selected(ID = 0, Value = 3)]
		[IO(Export = true)]
		public SinglePVAParamater SinglePVA
		{
			get;
			private set;
		}
		
		[Selected(ID = 0, Value = 4)]
		[IO(Export = true)]
		public FloatEasingParamater SingleEasing
		{
			get;
			private set;
		}

		[Selected(ID = 0, Value = 5)]
		[IO(Export = true)]
		public Vector3DFCurveParameter FCurve
		{
			get;
			private set;
		}

		internal ScaleValues()
		{
			Type = new Value.Enum<ParamaterType>(ParamaterType.Fixed);
			Fixed = new FixedParamater();
			PVA = new PVAParamater();
			Easing = new Vector3DEasingParamater(1.0f, 1.0f, 1.0f);
			//Easing.Start.X.SetCenterDirectly(1.0f);
			//Easing.Start.Y.SetCenterDirectly(1.0f);
			//Easing.Start.Z.SetCenterDirectly(1.0f);
			//Easing.Start.X.SetMaxDirectly(1.0f);
			//Easing.Start.Y.SetMaxDirectly(1.0f);
			//Easing.Start.Z.SetMaxDirectly(1.0f);
			//Easing.Start.X.SetMinDirectly(1.0f);
			//Easing.Start.Y.SetMinDirectly(1.0f);
			//Easing.Start.Z.SetMinDirectly(1.0f);
			//Easing.End.X.SetCenterDirectly(1.0f);
			//Easing.End.Y.SetCenterDirectly(1.0f);
			//Easing.End.Z.SetCenterDirectly(1.0f);
			//Easing.End.X.SetMaxDirectly(1.0f);
			//Easing.End.Y.SetMaxDirectly(1.0f);
			//Easing.End.Z.SetMaxDirectly(1.0f);
			//Easing.End.X.SetMinDirectly(1.0f);
			//Easing.End.Y.SetMinDirectly(1.0f);
			//Easing.End.Z.SetMinDirectly(1.0f);
			SinglePVA = new SinglePVAParamater();

			SingleEasing = new FloatEasingParamater(1.0f);
			//SingleEasing.Start.SetCenterDirectly(1.0f);
			//SingleEasing.Start.SetMaxDirectly(1.0f);
			//SingleEasing.Start.SetMinDirectly(1.0f);
			//SingleEasing.End.SetCenterDirectly(1.0f);
			//SingleEasing.End.SetMaxDirectly(1.0f);
			//SingleEasing.End.SetMinDirectly(1.0f);

			FCurve = new Vector3DFCurveParameter();
			FCurve.FCurve.X.DefaultValue = 1.0f;
			FCurve.FCurve.Y.DefaultValue = 1.0f;
			FCurve.FCurve.Z.DefaultValue = 1.0f;
		}

		public class FixedParamater
		{
			[Name(language = Language.Japanese, value = "拡大率")]
			[Description(language = Language.Japanese, value = "インスタンスの拡大率")]
			[Name(language = Language.English, value = "Scaling Factor")]
			[Description(language = Language.English, value = "Magnification of the instance")]
			public Value.Vector3D Scale
			{
				get;
				private set;
			}

			internal FixedParamater()
			{
				Scale = new Value.Vector3D(1, 1, 1);
			}
		}

		public class PVAParamater
		{
			[Name(language = Language.Japanese, value = "拡大率")]
			[Description(language = Language.Japanese, value = "インスタンスの拡大率")]
			[Name(language = Language.English, value = "Scaling Factor")]
			[Description(language = Language.English, value = "Magnification of the instance")]
			public Value.Vector3DWithRandom Scale
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "拡大速度")]
			[Description(language = Language.Japanese, value = "インスタンスの拡大速度")]
			[Name(language = Language.English, value = "Expansion\nSpeed")]
			[Description(language = Language.English, value = "The instance's initial rate of expansion")]
			public Value.Vector3DWithRandom Velocity
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "拡大加速度")]
			[Description(language = Language.Japanese, value = "インスタンスの初期拡大加速度")]
			[Name(language = Language.English, value = "Expansion\nAccel")]
			[Description(language = Language.English, value = "Acceleration of the instance's expansion rate")]
			public Value.Vector3DWithRandom Acceleration
			{
				get;
				private set;
			}

			internal PVAParamater()
			{
				Scale = new Value.Vector3DWithRandom(1, 1, 1);
				Velocity = new Value.Vector3DWithRandom(0, 0, 0);
				Acceleration = new Value.Vector3DWithRandom(0, 0, 0);
			}
		}

		public class SinglePVAParamater
		{
			[Name(language = Language.Japanese, value = "拡大率")]
			[Description(language = Language.Japanese, value = "インスタンスの拡大率")]
			[Name(language = Language.English, value = "Scaling Factor")]
			[Description(language = Language.English, value = "Magnification of the instance")]
			public Value.FloatWithRandom Scale
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "拡大速度")]
			[Description(language = Language.Japanese, value = "インスタンスの拡大速度")]
			[Name(language = Language.English, value = "Expansion\nSpeed")]
			[Description(language = Language.English, value = "The instance's initial rate of expansion")]
			public Value.FloatWithRandom Velocity
			{
				get;
				private set;
			}

			[Name(language = Language.Japanese, value = "拡大加速度")]
			[Description(language = Language.Japanese, value = "インスタンスの初期拡大加速度")]
			[Name(language = Language.English, value = "Expansion\nAccel")]
			[Description(language = Language.English, value = "Acceleration of the instance's expansion rat")]
			public Value.FloatWithRandom Acceleration
			{
				get;
				private set;
			}

			internal SinglePVAParamater()
			{
				Scale = new Value.FloatWithRandom(1);
				Velocity = new Value.FloatWithRandom(0);
				Acceleration = new Value.FloatWithRandom(0);
			}
		}

		public enum ParamaterType : int
		{
			[Name(value = "拡大率", language = Language.Japanese)]
			[Name(value = "Fixed Scale", language = Language.English)]
			Fixed = 0,
			[Name(value = "拡大率・速度・加速度", language = Language.Japanese)]
			[Name(value = "PVA", language = Language.English)]
			PVA = 1,
			[Name(value = "イージング", language = Language.Japanese)]
			[Name(value = "Easing", language = Language.English)]
			Easing = 2,
			[Name(value = "単一 拡大率・速度・加速度", language = Language.Japanese)]
			[Name(value = "PVA (Single)", language = Language.English)]
			SinglePVA = 3,
			[Name(value = "単一 イージング", language = Language.Japanese)]
			[Name(value = "Easing (Single)", language = Language.English)]
			SingleEasing = 4,
			[Name(value = "拡大率(Fカーブ)", language = Language.Japanese)]
			[Name(value = "F-Curve", language = Language.English)]
			FCurve = 5,
		}
	}
}

namespace EffekseerTool.Data
{
	public class SoundValues
	{
		[Selector(ID = 0)]
		public Value.Enum<ParamaterType> Type
		{
			get;
			private set;
		}

        [Selected(ID = 0, Value = 0)]
        [IO(Export = true)]
        public NoneParamater None
        {
            get;
            private set;
        }

		[Selected(ID = 0, Value = 1)]
		[IO(Export = true)]
		public SoundParamater Sound
		{
			get;
			private set;
		}

		internal SoundValues()
		{
            Type = new Value.Enum<ParamaterType>(ParamaterType.None);
            None = new NoneParamater();
            Sound = new SoundParamater();
		}

        public class NoneParamater
        {
            internal NoneParamater()
            {
            }
        }

        public class SoundParamater
        {
            [Name(language = Language.Japanese, value = "音データ")]
            [Description(language = Language.Japanese, value = "再生する音のWAVデータ")]
			[Name(language = Language.English, value = "Sound Data")]
			[Description(language = Language.English, value = "Using WAV files")]
            public Value.PathForSound Wave
            {
                get;
                private set;
            }
            [Name(value = "ボリューム", language = Language.Japanese)]
            [Description(language = Language.Japanese, value = "音の大きさ。\n範囲：0.0 ～ 1.0\n0で無音。1で原音の大きさ")]
			[Name(value = "Volume", language = Language.English)]
			[Description(language = Language.English, value = "Volume\nRange：0.0 ～ 1.0\n0 is silence, 1 is same as the original sound.")]
            public Value.FloatWithRandom Volume
            {
                get;
                private set;
            }
            [Name(value = "ピッチ", language = Language.Japanese)]
            [Description(language = Language.Japanese, value = "音の高さ（単位：オクターブ）。\n範囲：-4.0 ～ +4.0\n0.0で原音。+1.0で2倍。-1.0で1/2倍")]
			[Name(value = "Pitch", language = Language.English)]
			[Description(language = Language.English, value = "Pitch（Units：Octave)\nRange：-4.0 ～ +4.0\n0.0 is same as original, +1.0 doubled pitch, -1.0 half pitch.")]
            public Value.FloatWithRandom Pitch
            {
                get;
                private set;
            }

            [Selector(ID = 0)]
            [Name(value = "パンタイプ", language = Language.Japanese)]
            [Description(language = Language.Japanese, value = "パンタイプ")]
			[Name(value = "Panning Type", language = Language.English)]
			[Description(language = Language.English, value = "Panning Type")]
            public Value.Enum<ParamaterPanType> PanType
            {
                get;
                private set;
            }

            [Selected(ID = 0, Value = 0)]
            [Name(value = "パン", language = Language.Japanese)]
            [Description(language = Language.Japanese, value = "音の定位。\n範囲：-1.0 ～ +1.0\n0.0で中央。-1.0で左から。1.0で右から")]
			[Name(value = "Pan", language = Language.English)]
			[Description(language = Language.English, value = "Panning of sound.\nRange: -1.0 ～ +1.0\n0.0 is centered, -1.0 is left, 1.0 is right.")]
            public Value.FloatWithRandom Pan
            {
                get;
                private set;
            }

            [Selected(ID = 0, Value = 1)]
            [Name(value = "減衰距離", language = Language.Japanese)]
            [Description(language = Language.Japanese, value = "距離による減衰開始距離")]
			[Name(value = "Damping Range", language = Language.English)]
			[Description(language = Language.English, value = "Distance in which damping of the sound occurs")]
            public Value.Float Distance
            {
                get;
                private set;
            }

            [Name(value = "ディレイ", language = Language.Japanese)]
            [Description(language = Language.Japanese, value = "生成から再生開始までのフレーム数")]
			[Name(value = "Delay", language = Language.English)]
			[Description(language = Language.English, value = "Duration in frames between when the instance is spawned, and when the sound plays")]
            public Value.IntWithRandom Delay
            {
                get;
                private set;
            }

            internal SoundParamater()
			{
				Wave = new Value.PathForSound(Resources.GetString("SoundFilter"), true, "");
                Volume = new Value.FloatWithRandom(1.0f, 1.0f, 0.0f, DrawnAs.CenterAndAmplitude, 0.1f);
                Pitch = new Value.FloatWithRandom(0.0f, 2.0f, -2.0f, DrawnAs.CenterAndAmplitude, 0.1f);
                PanType = new Value.Enum<ParamaterPanType>(ParamaterPanType.Sound2D);
                Pan = new Value.FloatWithRandom(0.0f, 1.0f, -1.0f, DrawnAs.CenterAndAmplitude, 0.1f);
                Distance = new Value.Float(10.0f);
                Delay = new Value.IntWithRandom(0, int.MaxValue, 0);
			}
        }

        public enum ParamaterPanType : int
        {
            [Name(value = "2D", language = Language.Japanese)]
			[Name(value = "2D", language = Language.English)]
            Sound2D = 0,
            [Name(value = "3D", language = Language.Japanese)]
			[Name(value = "3D", language = Language.English)]
            Sound3D = 1,
        }

        public enum ParamaterType : int
        {
            [Name(value = "無し", language = Language.Japanese)]
			[Name(value = "Disabled", language = Language.English)]
            None = 0,
            [Name(value = "有り", language = Language.Japanese)]
			[Name(value = "Enabled", language = Language.English)]
            Use = 1,
        }
	}
}

namespace EffekseerTool.Data.Value
{
	public class Boolean
	{
		bool _value = false;

		public bool Value
		{
			get
			{
				return GetValue();
			}

			set
			{
				SetValue(value);
			}
		}

		public event ChangedValueEventHandler OnChanged;

		public bool DefaultValue { get; private set; }

		public bool IsValueChangedFromDefault
		{
			get { return Value != DefaultValue; }
		}

		internal Boolean(bool value = false)
		{
			_value = value;
			DefaultValue = _value;
		}

		public bool GetValue()
		{
			return _value;
		}

		public void SetValue(bool value)
		{
			if (value == _value) return;

			bool old_value = _value;
			bool new_value = value;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_value = new_value;

					if (OnChanged != null)
					{ 
						OnChanged(this, new ChangedValueEventArgs(new_value, ChangedValueType.Execute));
					}
				},
				() =>
				{
					_value = old_value;

					if (OnChanged != null)
					{
						OnChanged(this, new ChangedValueEventArgs(old_value, ChangedValueType.Unexecute));
					}
				});

			Command.CommandManager.Execute(cmd);
		}

		public void SetValueDirectly(bool value)
		{
			if (_value == value) return;

			_value = value;

			if (OnChanged != null)
			{
				OnChanged(this, new ChangedValueEventArgs(_value, ChangedValueType.Execute));
			}
		}

		public static implicit operator bool(Boolean value)
		{
			return value._value;
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class Color
	{
		public Int R
		{
			get;
			private set;
		}

		public Int G
		{
			get;
			private set;
		}

		public Int B
		{
			get;
			private set;
		}

		public Int A
		{
			get;
			private set;
		}

		public bool IsValueChangedFromDefault
		{
			get { return 
					R.IsValueChangedFromDefault || 
					G.IsValueChangedFromDefault || 
					B.IsValueChangedFromDefault || 
					A.IsValueChangedFromDefault; }
		}

		internal Color(
			byte r = 0,
			byte g = 0,
			byte b = 0,
			byte a = 0,
			byte r_max = byte.MaxValue,
			byte r_min = byte.MinValue,
			byte g_max = byte.MaxValue,
			byte g_min = byte.MinValue,
			byte b_max = byte.MaxValue,
			byte b_min = byte.MinValue,
			byte a_max = byte.MaxValue,
			byte a_min = byte.MinValue)
		{
			R = new Int(r, r_max, r_min);
			G = new Int(g, g_max, g_min);
			B = new Int(b, b_max, b_min);
			A = new Int(a, a_max, a_min);
		}

		public void SetValue(int r, int g, int b, int a = -1, bool isCombined = false)
		{
			if (
				r == R.GetValue() &&
				g == G.GetValue() &&
				b == B.GetValue() &&
				a == A.GetValue()) return;

			int old_r = R.GetValue();
			int new_r = r;

			int old_g = G.GetValue();
			int new_g = g;

			int old_b = B.GetValue();
			int new_b = b;

			int old_a = A.GetValue();
			int new_a = a;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					R.SetValueDirectly(new_r);
					G.SetValueDirectly(new_g);
					B.SetValueDirectly(new_b);
					A.SetValueDirectly(new_a);
				},
				() =>
				{
					R.SetValueDirectly(old_r);
					G.SetValueDirectly(old_g);
					B.SetValueDirectly(old_b);
					A.SetValueDirectly(old_a);
				},
				this,
				isCombined);

			Command.CommandManager.Execute(cmd);
		}

		public static implicit operator byte[](Color value)
		{
			byte[] values = new byte[sizeof(byte) * 4] { (byte)value.R, (byte)value.G, (byte)value.B, (byte)value.A };
			return values;
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class ColorWithRandom
	{
		public IntWithRandom R
		{
			get;
			private set;
		}

		public IntWithRandom G
		{
			get;
			private set;
		}

		public IntWithRandom B
		{
			get;
			private set;
		}

		public IntWithRandom A
		{
			get;
			private set;
		}

		public DrawnAs DrawnAs
		{
			get;
			set;
		}
		
		private ColorSpace _colorSpace;
		public ColorSpace ColorSpace
		{
			get
			{
				return GetColorSpace();
			}
		}

		public bool IsValueChangedFromDefault
		{
			get { return 
					R.IsValueChangedFromDefault ||
					G.IsValueChangedFromDefault || 
					B.IsValueChangedFromDefault || 
					A.IsValueChangedFromDefault || 
					DrawnAs != DefaultDrawnAs || 
					ColorSpace != DefaultColorSpace; }
		}

		public ColorWithRandom Link
		{
			get;
			set;
		}

		internal DrawnAs DefaultDrawnAs { get; private set; }

		internal ColorSpace DefaultColorSpace { get; private set; }

		public event ChangedValueEventHandler OnChangedColorSpace;

		public void SetColorSpace(ColorSpace colorSpace, bool isColorConverted, bool isCombined)
		{
			ColorSpace oldval = ColorSpace;
			ColorSpace newval = colorSpace;
			
			if (oldval == newval) return;

			int old_min_r = R.GetMin();
			int old_min_g = G.GetMin();
			int old_min_b = B.GetMin();
			int old_min_a = A.GetMin();
			int old_max_r = R.GetMax();
			int old_max_g = G.GetMax();
			int old_max_b = B.GetMax();
			int old_max_a = A.GetMax();

			color color_min;
			color color_max;

			color_max.R = old_max_r;
			color_max.G = old_max_g;
			color_max.B = old_max_b;
			color_min.R = old_min_r;
			color_min.G = old_min_g;
			color_min.B = old_min_b;

			if (isColorConverted)
			{
				if (newval == ColorSpace.HSVA)
				{
					color_min = RGBToHSV(color_min);
					color_max = RGBToHSV(color_max);
				}
				else
				{
					color_min = HSVToRGB(color_min);
					color_max = HSVToRGB(color_max);
				}
			}

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_colorSpace = newval;

					R.SetMaxDirectly(color_max.R);
					G.SetMaxDirectly(color_max.G);
					B.SetMaxDirectly(color_max.B);

					R.SetMinDirectly(color_min.R);
					G.SetMinDirectly(color_min.G);
					B.SetMinDirectly(color_min.B);

					CallChangedColorSpace(false, ChangedValueType.Execute);
				},
				() =>
				{
					R.SetMaxDirectly(old_max_r);
					G.SetMaxDirectly(old_max_g);
					B.SetMaxDirectly(old_max_b);

					R.SetMinDirectly(old_min_r);
					G.SetMinDirectly(old_min_g);
					B.SetMinDirectly(old_min_b);

					_colorSpace = oldval;
					CallChangedColorSpace(false, ChangedValueType.Unexecute);
				},
				this,
				isCombined);

			Command.CommandManager.Execute(cmd);
		}

		public ColorSpace GetColorSpace()
		{
			return _colorSpace;
		}

		protected void CallChangedColorSpace(object value, ChangedValueType type)
		{
			if (OnChangedColorSpace != null)
			{
				OnChangedColorSpace(this, new ChangedValueEventArgs(value, type));
			}
		}

		internal ColorWithRandom(
			byte r = 0,
			byte g = 0,
			byte b = 0,
			byte a = 0,
			byte r_max = byte.MaxValue,
			byte r_min = byte.MinValue,
			byte g_max = byte.MaxValue,
			byte g_min = byte.MinValue,
			byte b_max = byte.MaxValue,
			byte b_min = byte.MinValue,
			byte a_max = byte.MaxValue,
			byte a_min = byte.MinValue,
			DrawnAs drawnas = Data.DrawnAs.CenterAndAmplitude,
			ColorSpace colorSpace = Data.ColorSpace.RGBA)
		{
			R = new IntWithRandom(r, r_max, r_min);
			G = new IntWithRandom(g, g_max, g_min);
			B = new IntWithRandom(b, b_max, b_min);
			A = new IntWithRandom(a, a_max, a_min);
			DrawnAs = drawnas;
			SetColorSpace(colorSpace, false, false);

			DefaultDrawnAs = DrawnAs;
			DefaultColorSpace = ColorSpace;
		}

		public void SetMin(int r, int g, int b, int a = -1, bool isCombined = false)
		{
			if (
				r == R.GetMin() &&
				g == G.GetMin() &&
				b == B.GetMin() &&
				a == A.GetMin()) return;

			int old_r = R.GetMin();
			int new_r = r;

			int old_g = G.GetMin();
			int new_g = g;

			int old_b = B.GetMin();
			int new_b = b;

			int old_a = A.GetMin();
			int new_a = a;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					R.SetMinDirectly(new_r);
					G.SetMinDirectly(new_g);
					B.SetMinDirectly(new_b);
					A.SetMinDirectly(new_a);

					R.CallChanged(new_r, ChangedValueType.Execute);
					G.CallChanged(new_g, ChangedValueType.Execute);
					B.CallChanged(new_b, ChangedValueType.Execute);
					A.CallChanged(new_a, ChangedValueType.Execute);
				},
				() =>
				{
					R.SetMinDirectly(old_r);
					G.SetMinDirectly(old_g);
					B.SetMinDirectly(old_b);
					A.SetMinDirectly(old_a);

					R.CallChanged(old_r, ChangedValueType.Unexecute);
					G.CallChanged(old_g, ChangedValueType.Unexecute);
					B.CallChanged(old_b, ChangedValueType.Unexecute);
					A.CallChanged(old_a, ChangedValueType.Unexecute);
				},
				this,
				isCombined);

			Command.CommandManager.Execute(cmd);
		}

		public void SetMax(int r, int g, int b, int a = -1, bool isCombined = false)
		{
			if (
				r == R.GetMax() &&
				g == G.GetMax() &&
				b == B.GetMax() &&
				a == A.GetMax()) return;

			int old_r = R.GetMax();
			int new_r = r;

			int old_g = G.GetMax();
			int new_g = g;

			int old_b = B.GetMax();
			int new_b = b;

			int old_a = A.GetMax();
			int new_a = a;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					R.SetMaxDirectly(new_r);
					G.SetMaxDirectly(new_g);
					B.SetMaxDirectly(new_b);
					A.SetMaxDirectly(new_a);

					R.CallChanged(new_r, ChangedValueType.Execute);
					G.CallChanged(new_g, ChangedValueType.Execute);
					B.CallChanged(new_b, ChangedValueType.Execute);
					A.CallChanged(new_a, ChangedValueType.Execute);
				},
				() =>
				{
					R.SetMaxDirectly(old_r);
					G.SetMaxDirectly(old_g);
					B.SetMaxDirectly(old_b);
					A.SetMaxDirectly(old_a);

					R.CallChanged(old_r, ChangedValueType.Unexecute);
					G.CallChanged(old_g, ChangedValueType.Unexecute);
					B.CallChanged(old_b, ChangedValueType.Unexecute);
					A.CallChanged(old_a, ChangedValueType.Unexecute);
				},
				this,
				isCombined);

			Command.CommandManager.Execute(cmd);
		}

		public void ChangeColorSpace(ColorSpace colorSpace, bool link = false)
		{
			Command.CommandManager.StartCollection();

			SetColorSpace(colorSpace, true, false);
			CallChangedColorSpace(true, ChangedValueType.Execute);
			
			if (link && Link != null)
			{
				Link.ChangeColorSpace(colorSpace, false);
			}

			Command.CommandManager.EndCollection();
		}

		public static implicit operator byte[](ColorWithRandom value)
		{
			byte[] values = new byte[sizeof(byte) * 10]
			{
				(byte)value.ColorSpace,
				(byte)0,	// reserved
				(byte)value.R.Max,
				(byte)value.G.Max,
				(byte)value.B.Max,
				(byte)value.A.Max,
				(byte)value.R.Min,
				(byte)value.G.Min,
				(byte)value.B.Min,
				(byte)value.A.Min
			};

			return values;
		}

		struct color
		{
			public int R;
			public int G;
			public int B;
		}

		static color RGBToHSV(color rgb)
		{
			double max;
			double min;
			double R, G, B, H, S, V;

			R = (double)rgb.R / 255.0;
			G = (double)rgb.G / 255.0;
			B = (double)rgb.B / 255.0;

			if (R >= G && R >= B)
			{
				max = R;
				min = (G < B) ? G : B;
			}
			else if (G >= R && G >= B)
			{
				max = G;
				min = (R < B) ? R : B;
			}
			else
			{
				max = B;
				min = (R < G) ? R : G;
			}
			if (R == G && G == B)
			{
				H = 0.0f;
			}
			else if (max == R)
			{
				H = 60 * (G - B) / (max - min);
			}
			else if (max == G)
			{
				H = 60 * (B - R) / (max - min) + 120;
			}
			else
			{
				H = 60 * (R - G) / (max - min) + 240;
			}
			if (H < 0.0f)
			{
				H += 360.0f;
			}
			if (max == 0.0f)
			{
				S = 0.0f;
			}
			else
			{
				S = (max - min) / max;
			}
			V = max;

			color ret = new color();
			ret.R = (int)Math.Round(H / 360 * 252, MidpointRounding.AwayFromZero);
			ret.G = (int)Math.Round(S * 255, MidpointRounding.AwayFromZero);
			ret.B = (int)Math.Round(V * 255, MidpointRounding.AwayFromZero);
			return ret;
		}

		static color HSVToRGB(color hsv)
		{
			int H = hsv.R, S = hsv.G, V = hsv.B;
			int i, R = 0, G = 0, B = 0, f, p, q, t;

			i = H / 42 % 6;
			f = H % 42 * 6;
			p = (V * (256 - S)) >> 8;
			q = (V * (256 - ((S * f) >> 8))) >> 8;
			t = (V * (256 - ((S * (252 - f)) >> 8))) >> 8;
			if (p < 0) p = 0;
			if (p > 255) p = 255;
			if (q < 0) q = 0;
			if (q > 255) q = 255;
			if (t < 0) t = 0;
			if (t > 255) t = 255;

			switch (i)
			{
				case 0: R = V; G = t; B = p; break;
				case 1: R = q; G = V; B = p; break;
				case 2: R = p; G = V; B = t; break;
				case 3: R = p; G = q; B = V; break;
				case 4: R = t; G = p; B = V; break;
				case 5: R = V; G = p; B = q; break;
			}

			color ret = new color();
			ret.R = R;
			ret.G = G;
			ret.B = B;
			return ret;
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class Enum<T> : EnumBase where T : struct, IComparable, IFormattable, IConvertible
	{
		T _value = default(T);

		public T Value
		{
			get
			{
				return GetValue();
			}

			set
			{
				SetValue(value);
			}
		}

		public T DefaultValue { get; private set; }

		public bool IsValueChangedFromDefault
		{
			get { return GetValueAsInt() != GetDefaultValueAsInt(); }
		}

		internal Enum(T value = default(T))
		{
			_value = value;
			DefaultValue = value;
		}

		public T GetValue()
		{
			return _value;
		}

		public override int GetValueAsInt()
		{
			object v = _value;
			return (int)v;
		}

		public override int GetDefaultValueAsInt()
		{
			object v = DefaultValue;
			return (int)v;
		}

		public void SetValue(T value)
		{
			if (value.ToString() == _value.ToString()) return;

			var old_value = _value;
			var new_value = value;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_value = new_value;
					Change(new_value, ChangedValueType.Execute);
				},
				() =>
				{
					_value = old_value;
					Change(old_value, ChangedValueType.Unexecute);
				});

			Command.CommandManager.Execute(cmd);
		}

		public override void SetValue(int value)
		{
			object v = value;
			SetValue((T)v);
		}

		public void SetValueDirectly(T value)
		{
			if (_value.ToString() == value.ToString()) return;

			_value = value;
			Change(_value, ChangedValueType.Execute);
		}

		public override void SetValueDirectly(int value)
		{
			object v = value;
			SetValueDirectly((T)v);
		}

		public override Type GetEnumType()
		{
			return typeof(T);
		}

		public static implicit operator T(Enum<T> value)
		{
			return value._value;
		}

		public static implicit operator byte[](Enum<T> value)
		{
			var n = value.GetValueAsInt();
			return BitConverter.GetBytes(n);
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public abstract class EnumBase
	{
		public abstract int GetValueAsInt();
		public abstract int GetDefaultValueAsInt();
		public abstract void SetValue(int value);
		public abstract void SetValueDirectly(int value);
		public event ChangedValueEventHandler OnChanged;

		public abstract Type GetEnumType();

		protected void Change(object value, ChangedValueType type)
		{
			if (OnChanged != null)
			{
				OnChanged(this, new ChangedValueEventArgs(value, type));
			}
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public enum FCurveEdge
	{
		[Name(language = Language.Japanese, value = "一定")]
		[Name(language = Language.English, value = "Constant")]
		Constant = 0,
		[Name(language = Language.Japanese, value = "ループ")]
		[Name(language = Language.English, value = "Loop")]
		Loop = 1,
		[Name(language = Language.Japanese, value = "逆ループ")]
		[Name(language = Language.English, value = "Loop inversely")]
		LoopInversely = 2,
	}

	public enum FCurveInterpolation
	{
		[Name(language = Language.Japanese, value = "ベジェ")]
		[Name(language = Language.English, value = "Bezier")]
		Bezier = 0,
		[Name(language = Language.Japanese, value = "線形")]
		[Name(language = Language.English, value = "Linear")]
		Linear = 1,
	}

	public interface IFCurve
	{
		Enum<FCurveEdge> StartType { get; }
		Enum<FCurveEdge> EndType { get; }
		void RemoveKey(IFCurveKey key);
		float GetValue(int frame);
		IEnumerable<IFCurveKey> Keys { get; }
		Float OffsetMax { get; }
		Float OffsetMin { get; }
		Int Sampling { get; }
	}

	public class FCurve<T> : IFCurve where T : struct, IComparable<T>, IEquatable<T>
	{
        float ToFloat(T v)
        {
            var o = (object)v;

            if (o is float) return (float)o;

            if (v is byte)
            {
                var b = (byte)o;
                return (float)b;
            }

            throw new NotImplementedException();
        }

        public event ChangedValueEventHandler OnChanged;

		List<FCurveKey<T>> keys = new List<FCurveKey<T>>();

		public IEnumerable<IFCurveKey> Keys
		{
			get
			{
				return (IEnumerable<IFCurveKey>)keys;
			}
		}

		public Enum<FCurveEdge> StartType { get; private set; }

		public Enum<FCurveEdge> EndType { get; private set; }

		public Float OffsetMax { get; private set; }
		public Float OffsetMin { get; private set; }

		public Int Sampling { get; private set; }

		public float DefaultValueRangeMax { get; set; }
		public float DefaultValueRangeMin { get; set; }

		public T DefaultValue { get; set; }

		public FCurve(T defaultValue)
		{
			DefaultValueRangeMax = 100.0f;
			DefaultValueRangeMin = -100.0f;

			this.DefaultValue = defaultValue;
			StartType = new Enum<FCurveEdge>(FCurveEdge.Constant);
			EndType = new Enum<FCurveEdge>(FCurveEdge.Constant);

			OffsetMax = new Float();
			OffsetMin = new Float();

			Sampling = new Int(5, int.MaxValue, 1, 1);
		}

		public void SetKeys(FCurveKey<T>[] keys)
		{
			var new_value = keys.ToList();
			var old_value = this.keys.ToList();

			var cmd = new Command.DelegateCommand(
			() =>
			{
				this.keys = new_value;

				foreach (var key in keys)
				{
					key.OnChangedKey += OnChangedKey;
				}

				if (OnChanged != null)
				{
					OnChanged(this, new ChangedValueEventArgs(new_value, ChangedValueType.Execute));
				}
			},
			() =>
			{
				this.keys = old_value;

				foreach (var key in keys)
				{
					key.OnChangedKey -= OnChangedKey;
				}

				if (OnChanged != null)
				{
					OnChanged(this, new ChangedValueEventArgs(old_value, ChangedValueType.Unexecute));
				}
			});

			Command.CommandManager.Execute(cmd);
		}

		public void AddKey(FCurveKey<T> key)
		{
			if (keys.Contains(key)) return;

			var old_value = keys;
			var new_value = new List<FCurveKey<T>>();
			new_value.AddRange(keys.Concat( new[] {key}));
			SortKeys(new_value);
			
			var cmd = new Command.DelegateCommand(
				() =>
				{
					keys = new_value;
					key.OnChangedKey += OnChangedKey;

					if (OnChanged != null)
					{
						OnChanged(this, new ChangedValueEventArgs(new_value, ChangedValueType.Execute));
					}
				},
				() =>
				{
					keys = old_value;
					key.OnChangedKey -= OnChangedKey;

					if (OnChanged != null)
					{
						OnChanged(this, new ChangedValueEventArgs(old_value, ChangedValueType.Unexecute));
					}
				});

			Command.CommandManager.Execute(cmd);
		}

		public void RemoveKey(IFCurveKey key)
		{
			var k = key as FCurveKey<T>;
			if (k != null)
			{
				RemoveKey(k);
			}
		}

		public void RemoveKey(FCurveKey<T> key)
		{
			if (!keys.Contains(key)) return;

			var old_value = keys;
			var new_value = new List<FCurveKey<T>>();
			new_value.AddRange(keys.Except(new[] { key }));

			var cmd = new Command.DelegateCommand(
				() =>
				{
					keys = new_value;
					key.OnChangedKey -= OnChangedKey;

					if (OnChanged != null)
					{
						OnChanged(this, new ChangedValueEventArgs(new_value, ChangedValueType.Execute));
					}
				},
				() =>
				{
					keys = old_value;
					key.OnChangedKey += OnChangedKey;

					if (OnChanged != null)
					{
						OnChanged(this, new ChangedValueEventArgs(old_value, ChangedValueType.Unexecute));
					}
				});

			Command.CommandManager.Execute(cmd);
		}

		void OnChangedKey(FCurveKey<T> key)
		{
			keys.Sort(new KeyComparer());
		}

		public byte[] GetBytes(float mul = 1.0f)
		{
			var freq = Sampling.Value;
			List<byte[]> data = new List<byte[]>();

			data.Add(BitConverter.GetBytes((int)StartType.Value));
			data.Add(BitConverter.GetBytes((int)EndType.Value));
			data.Add(BitConverter.GetBytes(OffsetMax.Value));
			data.Add(BitConverter.GetBytes(OffsetMin.Value));

			if (keys.Count > 0)
			{
				var len = keys.Last().Frame - keys.First().Frame;
				data.Add(BitConverter.GetBytes(keys.First().Frame));
				data.Add(BitConverter.GetBytes(len));
				data.Add(BitConverter.GetBytes(freq));

				int count = 0;
				if (len % freq > 0)
				{
					count = len / freq + 2;
				}
				else
				{
					count = len / freq + 1;
				}

				data.Add(BitConverter.GetBytes(count));

				if (typeof(T) == typeof(float))
				{
					for (int f = keys.First().Frame; f < keys.Last().Frame; f += freq)
					{
						var v = GetValue(f) * mul;
						data.Add(BitConverter.GetBytes(v));
					}

					{
						var v = GetValue(keys.Last().Frame) * mul;
						data.Add(BitConverter.GetBytes(v));
					}
				}
				else if (typeof(T) == typeof(byte))
				{
					for (int f = keys.First().Frame; f < keys.Last().Frame; f += freq)
					{
						var v = GetValue(f) * mul;
						data.Add(BitConverter.GetBytes(v));
					}

					{
						var v = GetValue(keys.Last().Frame) * mul;
						data.Add(BitConverter.GetBytes(v));
					}
				}
				
			}
			else
			{
				data.Add(BitConverter.GetBytes(0));
				data.Add(BitConverter.GetBytes(0));
				data.Add(BitConverter.GetBytes(0));
				data.Add(BitConverter.GetBytes(0));
			}

			return data.SelectMany(_ => _).ToArray();
		}

		public void AddKeyDirectly(FCurveKey<T> key)
		{
			if (keys.Contains(key)) return;
			keys.Add(key);
			key.OnChangedKey += OnChangedKey;
			SortKeys(keys);
		}

		public float GetValue(int frame)
		{
			var begin = StartType.Value;
			var end = EndType.Value;

			int lInd = 0;
			int rInd = (int)keys.Count- 1;

			if (keys.Count == 0)
            {
                return ToFloat(DefaultValue);
            }

            int length = keys[rInd].Frame - keys[lInd].Frame;

			if (keys[rInd].Frame <= frame)
			{
				if (end == FCurveEdge.Constant)
				{
					return ToFloat(keys[rInd].Value);
				}
				else if (end == FCurveEdge.Loop)
				{
					frame = (frame - keys[rInd].Frame) % length + keys[lInd].Frame;
				}
				else if (end == FCurveEdge.LoopInversely)
				{
					frame = (length - (frame - keys[rInd].Frame) % length) + keys[lInd].Frame;
				}
			}

			if (frame <= keys[lInd].Frame)
			{
				if (begin == FCurveEdge.Constant)
				{
					return ToFloat(keys[lInd].Value);
				}
				else if (begin == FCurveEdge.Loop)
				{
					frame = (keys[lInd].Frame - frame) % length + keys[lInd].Frame;
				}
				else if (begin == FCurveEdge.LoopInversely)
				{
					frame = (length - (keys[lInd].Frame - frame) % length) + keys[lInd].Frame;
				}
			}

			while (true)
			{
				int mInd = (lInd + rInd) / 2;

				/* lIndとrIndの間 */
				if (mInd == lInd)
				{
					if (keys[lInd].InterpolationType.Value == FCurveInterpolation.Linear)
					{
						float subF = (float)(keys[rInd].Frame - keys[lInd].Frame);
						var subV = ToFloat(keys[rInd].Value) - ToFloat(keys[lInd].Value);

						if (subF == 0) return ToFloat(keys[lInd].Value);

						return subV / (float)(subF) * (float)(frame - keys[lInd].Frame) + ToFloat(keys[lInd].Value);
					}
					else if (keys[lInd].InterpolationType.Value == FCurveInterpolation.Bezier)
					{
						if (keys[lInd].Frame == frame) return ToFloat(keys[lInd].Value);

						float[] k1 = new float[2];
						float[] k1rh = new float[2];
						float[] k2lh = new float[2];
						float[] k2 = new float[2];

						k1[0] = keys[lInd].Frame;
						k1[1] = ToFloat(keys[lInd].Value);

						k1rh[0] = keys[lInd].RightX;
						k1rh[1] = keys[lInd].RightY;

						k2lh[0] = keys[rInd].LeftX;
						k2lh[1] = keys[rInd].LeftY;

						k2[0] = keys[rInd].Frame;
						k2[1] = ToFloat(keys[rInd].Value);

						NormalizeValues(k1, k1rh, k2lh, k2);
						float t = 0;
						var getT = CalcT(frame, k1[0], k1rh[0], k2lh[0], k2[0], out t);
						if (getT)
						{
							return CalcBezierValue(k1[1], k1rh[1], k2lh[1], k2[1], t);
						}
						else
						{
							return 0;
						}
					}
				}

				if (keys[mInd].Frame <= frame)
				{
					lInd = mInd;
				}
				else if (frame <= keys[mInd].Frame)
				{
					rInd = mInd;
				}
			}
		}

		/// <summary>
		/// キーをソートする。
		/// </summary>
		/// <param name="keys"></param>
		/// <returns></returns>
		public static bool SortKeys(ICollection<FCurveKey<T>> keys)
		{
			if (keys is List<FCurveKey<T>>)
			{
				var ks = keys as List<FCurveKey<T>>;
				ks.Sort(new KeyComparer());
				return true;
			}

			return false;
		}

		/// <summary>
		/// 三乗根を求める。
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		static float Sqrt3(float value)
		{
			if (value == 0)
			{
				return 0;
			}
			else if (value > 0)
			{
				return (float)Math.Pow(value, 1.0 / 3.0);
			}
			else
			{
				return -(float)Math.Pow(-value, 1.0 / 3.0);
			}
		}

		/// <summary>
		/// 平方根を求める。
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		static float Sqrt(float value)
		{
			return (float)Math.Sqrt(value);
		}

		/// <summary>
		/// 正しく値を求めれるように補正を行う。
		/// </summary>
		/// <param name="k1"></param>
		/// <param name="k1rh"></param>
		/// <param name="k2lh"></param>
		/// <param name="k2"></param>
		static void NormalizeValues(float[] k1, float[] k1rh, float[] k2lh, float[] k2)
		{
			var h1 = new float[2];
			var h2 = new float[2];

			// 傾きを求める
			h1[0] = k1[0] - k1rh[0];
			h1[1] = k1[1] - k1rh[1];

			h2[0] = k2[0] - k2lh[0];
			h2[1] = k2[1] - k2lh[1];

			// キーフレーム間の長さ
			var len = k2[0] - k1[0];

			// キー1の右手の長さ
			var lenR = Math.Abs(h1[0]);

			// キー2の左手の長さ
			var lenL = Math.Abs(h2[0]);

			// 長さがない
			if (lenR == 0 && lenL == 0) return;

			// 手が重なっている場合、補正
			if ((lenR + lenL) > len)
			{
				var f = len / (lenR + lenL);

				k1rh[0] = (k1[0] - f * h1[0]);
				k1rh[1] = (k1[1] - f * h1[1]);

				k2lh[0] = (k2[0] - f * h2[0]);
				k2lh[1] = (k2[1] - f * h2[1]);
			}
		}

		/// <summary>
		/// 二次方程式の解を求める。
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <param name="r1"></param>
		/// <param name="r2"></param>
		static void QuadraticFormula(float a, float b, float c, out float r1, out float r2)
		{
			r1 = float.NaN;
			r2 = float.NaN;

			if (a != 0.0)
			{
				float p = b * b - 4 * a * c;

				if (p > 0)
				{
					p = Sqrt(p);
					r1 = (-b - p) / (2 * a);
					r2 = (-b + p) / (2 * a);
				}
				else if (p == 0)
				{
					r1 = -b / (2 * a);
				}
			}
			else if (b != 0.0)
			{
				r1 = -c / b;
			}
			else if (c == 0.0f)
			{
				r1 = 0.0f;
			}
		}

		/// <summary>
		/// 三次方程式の解を求める。
		/// </summary>
		/// <param name="frame"></param>
		/// <param name="k1X"></param>
		/// <param name="k1rhX"></param>
		/// <param name="k2lhX"></param>
		/// <param name="k2X"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		static bool CalcT(float frame, float k1X, float k1rhX, float k2lhX, float k2X, out float r)
		{
			Func<float, bool> isValid = (v) =>
				{
					if (float.IsNaN(v)) return false;

					const float small = -0.00000001f;
					const float big = 1.000001f;
					return (v >= small) && (v <= big);
				};

			var c3_ = k2X - k1X + 3.0f * (k1rhX - k2lhX);
			var c2_ = 3.0f * (k1X - 2.0f * k1rhX + k2lhX);
			var c1_ = 3.0f * (k1rhX - k1X);
			var c0_ = k1X - frame;
			
			if (c3_ != 0.0)
			{
				var c2 = c2_ / c3_;
				var c1 = c1_ / c3_;
				var c0 = c0_ / c3_;

				var p = c1 / 3.0f - c2 * c2 / (3.0f * 3.0f);
				var q = (2.0f * c2 * c2 * c2 / (3.0f*3.0f*3.0f) - c2 / 3.0f * c1 + c0) / 2.0f;
				var p3q2 = q * q + p * p * p;

				if (p3q2 > 0.0)
				{
					var t = Sqrt(p3q2);
					var u = Sqrt3(-q + t);
					var v = Sqrt3(-q - t);
					r = u + v - c2 / 3.0f;

					if (isValid(r)) return true;
					return false;
				}
				else if (p3q2 == 0.0)
				{
					var u = Sqrt3(-q);
					var v = Sqrt3(-q);
					r = u + v - c2 / 3.0f;
					if (isValid(r)) return true;

					u = -Sqrt3(-q);
					v = -Sqrt3(-q);
					r = u + v - c2 / 3.0f;
					if (isValid(r)) return true;

					return false;
				}
				else
				{
					// ビエタの解
					var phi = (float)Math.Acos(-q / Sqrt(-(p * p * p)));
					var pd3 = (float)Math.Cos(phi / 3);
					var rmp = Sqrt(-p);

					r = 2.0f * rmp * (float)Math.Cos(phi / 3) - c2 / 3.0f;
					if (isValid(r)) return true;

					var q2 = Sqrt(3 - 3 * pd3 * pd3);

					//r = -rmp * (pd3 + q2) - a;
					r = 2.0f * rmp * (float)Math.Cos(phi / 3 + 2.0 * Math.PI / 3.0) - c2 / 3.0f;
					if (isValid(r)) return true;

					//r = -rmp * (pd3 - q2) - a;
					r = 2.0f * rmp * (float)Math.Cos(phi / 3 + 4.0 * Math.PI / 3.0) - c2 / 3.0f;
					if (isValid(r)) return true;
					return false;
				}
			}
			else
			{
				// 2次方程式のケース
				float r1;
				float r2;
				QuadraticFormula(c2_, c1_, c0_, out r1, out r2);

				r = r1;
				if (isValid(r)) return true;

				r = r2;
				if (isValid(r)) return true;

				r = 0.0f;
				return false;
			}
		}

		/// <summary>
		/// 三次方程式の解から値を求める。
		/// </summary>
		/// <param name="k1Y"></param>
		/// <param name="k1rhY"></param>
		/// <param name="k2lhY"></param>
		/// <param name="k2Y"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		static float CalcBezierValue(float k1Y, float k1rhY, float k2lhY, float k2Y, float t)
		{
			var c0 = k1Y;
			var c1 = 3.0f * (k1rhY - k1Y);
			var c2 = 3.0f * (k1Y - 2.0f * k1rhY + k2lhY);
			var c3 = k2Y - k1Y + 3.0f * (k1rhY - k2lhY);

			return c0 + t * c1 + t * t * c2 + t * t * t * c3;
		}



		public class KeyComparer : System.Collections.Generic.IComparer<FCurveKey<T>>
		{
			public int Compare(FCurveKey<T> key1, FCurveKey<T> key2)
			{
				if (key1.Frame > key2.Frame)
				{
					return 1;
				}
				else if (key1.Frame < key2.Frame)
				{
					return -1;
				}

				return key1.Value.CompareTo(key2.Value);
			}
		}
	}

	public interface IFCurveKey
	{
		int Frame { get; }
		float ValueAsFloat { get; }
		float LeftX { get; }
		float LeftY { get; }

		float RightX { get; }
		float RightY { get; }

		int FramePrevious { get; }
		float ValuePreviousAsFloat { get; }
		float LeftXPrevious { get; }
		float LeftYPrevious { get; }

		float RightXPrevious { get; }
		float RightYPrevious { get; }

		Enum<FCurveInterpolation> InterpolationType { get; }

		void Commit(bool isCombined = false);

		void SetFrame(int frame);
		void SetValueAsFloat(float value);
		void SetLeftX(float value);
		void SetLeftY(float value);
		void SetRightX(float value);
		void SetRightY(float value);

		void SetLeft(float x, float y);
		void SetRight(float x, float y);
	}

	public class FCurveKey<T> : IFCurveKey where T : struct, IComparable<T>, IEquatable<T>
	{
		int _frame = 0;
		T _value = default(T);
		float _left_x = 0;
		float _left_y = 0;
		float _right_x = 0;
		float _right_y = 0;


		int _frame_temp = 0;
		T _value_temp = default(T);
		float _left_x_temp = 0;
		float _left_y_temp = 0;
		float _right_x_temp = 0;
		float _right_y_temp = 0;

        float ToFloat(T v)
        {
            var o = (object)v;

            if (o is float) return (float)o;

            if (v is byte)
            {
                var b = (byte)o;
                return (float)b;
            }

            throw new NotImplementedException();
        }

        public event Action<FCurveKey<T>> OnChangedKey;

		public int Frame
		{
			get
			{
				return _frame_temp;
			}
		}

		public T Value
		{
			get
			{
				return _value_temp;
			}
		}

		public float ValueAsFloat
		{
			get
			{
                return ToFloat(_value_temp);
            }
		}

		public float LeftX
		{
			get
			{
				return _left_x_temp;
			}
		}

		public float LeftY
		{
			get
			{
				return _left_y_temp;
			}
		}

		public float RightX
		{
			get
			{
				return _right_x_temp;
			}
		}

		public float RightY
		{
			get
			{
				return _right_y_temp;
			}
		}

		public int FramePrevious
		{
			get
			{
				return _frame;
			}
		}

		public T ValuePrevious
		{
			get
			{
				return _value;
			}
		}

		public float ValuePreviousAsFloat
		{
			get
			{
                return ToFloat(_value);

            }
		}

		public float LeftXPrevious
		{
			get
			{
				return _left_x;
			}
		}

		public float LeftYPrevious
		{
			get
			{
				return _left_y;
			}
		}

		public float RightXPrevious
		{
			get
			{
				return _right_x;
			}
		}

		public float RightYPrevious
		{
			get
			{
				return _right_y;
			}
		}

		public void Commit(bool isCombined = false)
		{
			if (_frame == _frame_temp && 
				_value.Equals(_value_temp) &&
				_left_x.Equals(_left_x_temp) &&
				_left_y.Equals(_left_y_temp) &&
				_right_x.Equals(_right_x_temp) &&
				_right_y.Equals(_right_y_temp)) return;
		
			var old_frame = _frame;
			var new_frame = _frame_temp;

			var old_value = _value;
			var new_value = _value_temp;

			var old_left_x = _left_x;
			var new_left_x = _left_x_temp;
			var old_left_y = _left_y;
			var new_left_y = _left_y_temp;

			var old_right_x = _right_x;
			var new_right_x = _right_x_temp;
			var old_right_y = _right_y;
			var new_right_y = _right_y_temp;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_frame = new_frame;
					_value = new_value;

					_left_x = new_left_x;
					_left_y = new_left_y;
					_right_x = new_right_x;
					_right_y = new_right_y;
					
					_frame_temp = new_frame;
					_value_temp = new_value;

					_left_x_temp = new_left_x;
					_left_y_temp = new_left_y;
					_right_x_temp = new_right_x;
					_right_y_temp = new_right_y;
					

					if (OnChanged != null)
					{
						OnChanged(this, new ChangedValueEventArgs(new_value, ChangedValueType.Execute));
					}

					if (OnChangedKey != null)
					{
						OnChangedKey(this);
					}
				},
				() =>
				{
					_frame = old_frame;
					_value = old_value;

					_left_x = old_left_x;
					_left_y = old_left_y;
					_right_x = old_right_x;
					_right_y = old_right_y;

					_frame_temp = old_frame;
					_value_temp = old_value;

					_left_x_temp = old_left_x;
					_left_y_temp = old_left_y;
					_right_x_temp = old_right_x;
					_right_y_temp = old_right_y;

					if (OnChanged != null)
					{
						OnChanged(this, new ChangedValueEventArgs(old_value, ChangedValueType.Unexecute));
					}

					if (OnChangedKey != null)
					{
						OnChangedKey(this);
					}
				},
				this,
				isCombined);

			Command.CommandManager.Execute(cmd);
		}

		public Enum<FCurveInterpolation> InterpolationType { get; private set; }

		public void SetFrame(int frame)
		{
			if (frame == _frame_temp) return;
			var dif = frame - _frame_temp;
			_frame_temp = frame;

			_left_x_temp += dif;
			_right_x_temp += dif;

			if (OnChangedKey != null)
			{
				OnChangedKey(this);
			}
		}

		public void SetValue(T value)
		{
			if (value.CompareTo(_value_temp) == 0) return;
			var dif = ToFloat(value) - ToFloat(_value_temp);
			_value_temp = value;

			_left_y_temp += dif;
			_right_y_temp += dif;

			if (OnChangedKey != null)
			{
				OnChangedKey(this);
			}
		}

		public void SetValueAsFloat(float value)
		{
			if (typeof(T) == typeof(byte))
			{
				value = (float)Math.Round((double)value);
				value = Math.Max(0, value);
				value = Math.Min(255, value);
			}

			var valNow = ToFloat(_value_temp);
			if (valNow == value) return;
			
			var dif = value - valNow;
			_value_temp = (T)((object)value);

			_left_y_temp += dif;
			_right_y_temp += dif;

			if (OnChangedKey != null)
			{
				OnChangedKey(this);
			}
		}

		public void SetLeftX(float value)
		{
			if (value.CompareTo(_left_x_temp) == 0) return;

			_left_x_temp = value;
			if (_left_x_temp > _frame_temp)
			{
				_left_x_temp = _frame_temp;
			}

			// 傾き調整
			var _valueF = ToFloat(_value_temp);
			var dxR = _right_x_temp - _frame_temp;
			var dyR = _right_y_temp - ToFloat(_value_temp);

			var dxL = _left_x_temp - _frame_temp;
			var dyL = _left_y_temp - ToFloat(_value_temp);

			if (dxR != 0)
			{ 
				var h = dyR / dxR;
				_left_y_temp = _valueF + dxL * h;
			}
			else if (dxR == 0 && dyR != 0.0f)
			{
				_left_x_temp = _frame_temp;
			}
			
			if (OnChangedKey != null)
			{
				OnChangedKey(this);
			}
		}

		public void SetLeftY(float value)
		{
			if (value.CompareTo(_left_y_temp) == 0) return;

			_left_y_temp = value;

			// 傾き調整
			var _valueF = ToFloat(_value_temp);
			var dxR = _right_x_temp - _frame_temp;
			var dyR = _right_y_temp - ToFloat(_value_temp);

			var dxL = _left_x_temp - _frame_temp;
			var dyL = _left_y_temp - ToFloat(_value_temp);

			if (dxR != 0)
			{
				var h = dyR / dxR;
				_left_x_temp = _frame_temp + dyL / h;
			}
			else if (dxR == 0 && dyR != 0.0f)
			{
				_left_x_temp = _frame_temp;
			}

			if (OnChangedKey != null)
			{
				OnChangedKey(this);
			}
		}

		public void SetRightX(float value)
		{
			if (value.CompareTo(_right_x_temp) == 0) return;
			_right_x_temp = value;
			if (_right_x_temp < _frame_temp) _right_x_temp = _frame_temp;

			CorrectGradientRight();

			if (OnChangedKey != null)
			{
				OnChangedKey(this);
			}
		}

		public void SetRightY(float value)
		{
			if (value.CompareTo(_right_y_temp) == 0) return;
			_right_y_temp = value;

			CorrectGradientRight();

			if (OnChangedKey != null)
			{
				OnChangedKey(this);
			}
		}

		public void SetLeft(float x, float y)
		{
			if (x.CompareTo(_left_x_temp) == 0 &&
				y.CompareTo(_left_y_temp) == 0) return;

			_left_x_temp = x;
			if (_left_x_temp > _frame_temp)
			{
				_left_x_temp = _frame_temp;
			}
			_left_y_temp = y;

			var _valueF = ToFloat(_value_temp);
			var dxR = _right_x_temp - _frame_temp;
			var dyR = _right_y_temp - ToFloat(_value_temp);

			var dxL = _left_x_temp - _frame_temp;
			var dyL = _left_y_temp - ToFloat(_value_temp);

			var lenL = (float)Math.Sqrt(dxL * dxL + dyL * dyL);
			var lenR = (float)Math.Sqrt(dxR * dxR + dyR * dyR);

			if (lenL > 0 && lenR > 0)
			{
				_right_x_temp = _frame_temp - dxL / lenL * lenR;
				_right_y_temp = _valueF - dyL / lenL * lenR;
			}

			if (OnChangedKey != null)
			{
				OnChangedKey(this);
			}
		}

		public void SetRight(float x, float y)
		{
			if (x.CompareTo(_right_x_temp) == 0 &&
				y.CompareTo(_right_y_temp) == 0) return;

			_right_x_temp = x;
			if (_right_x_temp < _frame_temp) _right_x_temp = _frame_temp;
			_right_y_temp = y;

			CorrectGradientRight();

			if (OnChangedKey != null)
			{
				OnChangedKey(this);
			}
		}

		public void SetLeftDirectly(float x, float y)
		{
			_left_x = x;
			_left_x_temp = x;
			_left_y = y;
			_left_y_temp = y;
		}

		public void SetRightDirectly(float x, float y)
		{
			_right_x = x;
			_right_x_temp = x;
			_right_y = y;
			_right_y_temp = y;
		}

		/// <summary>
		/// 傾き調整
		/// </summary>
		void CorrectGradientRight()
		{
			var _valueF = ToFloat(_value_temp);
			var dxR = _right_x_temp - _frame_temp;
			var dyR = _right_y_temp - ToFloat(_value_temp);

			var dxL = _left_x_temp - _frame_temp;
			var dyL = _left_y_temp - ToFloat(_value_temp);

			var lenL = (float)Math.Sqrt(dxL * dxL + dyL * dyL);
			var lenR = (float)Math.Sqrt(dxR * dxR + dyR * dyR);

			if (lenL > 0 && lenR > 0)
			{
				_left_x_temp = _frame_temp - dxR / lenR * lenL;
				_left_y_temp = _valueF - dyR / lenR * lenL;
			}
		}

		public FCurveKey(int frame = 0, T value = default(T))
		{
			_frame = frame;
			_value = value;
			_frame_temp = frame;
			_value_temp = value;

			_left_x = frame;
			_left_x_temp = frame;
			_right_x = frame;
			_right_x_temp = frame;

			_left_y = ToFloat(value);
			_left_y_temp = ToFloat(value);
			_right_y = ToFloat(value);
			_right_y_temp = ToFloat(value);

			InterpolationType = new Enum<FCurveInterpolation>(FCurveInterpolation.Bezier);
		}

		public event ChangedValueEventHandler OnChanged;
	}
}

namespace EffekseerTool.Data.Value
{
	public class FCurveColorRGBA
	{
		public FCurve<byte> R { get; private set; }
		public FCurve<byte> G { get; private set; }
		public FCurve<byte> B { get; private set; }
		public FCurve<byte> A { get; private set; }

		public FCurveColorRGBA()
		{
			R = new FCurve<byte>(255);
			G = new FCurve<byte>(255);
			B = new FCurve<byte>(255);
			A = new FCurve<byte>(255);
		}

		public byte[] GetBytes(float mul = 1.0f)
		{
			List<byte[]> data = new List<byte[]>();
			data.Add(R.GetBytes(mul));
			data.Add(G.GetBytes(mul));
			data.Add(B.GetBytes(mul));
			data.Add(A.GetBytes(mul));
			return data.SelectMany(_ => _).ToArray();
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class FCurveVector2D
	{
		public FCurve<float> X { get; private set; }
		public FCurve<float> Y { get; private set; }

		public FCurveVector2D()
		{
			X = new FCurve<float>(0);
			Y = new FCurve<float>(0);
		}

		public byte[] GetBytes(float mul = 1.0f)
		{
			List<byte[]> data = new List<byte[]>();
			data.Add(X.GetBytes(mul));
			data.Add(Y.GetBytes(mul));
			return data.SelectMany(_ => _).ToArray();
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class FCurveVector3D
	{
		public FCurve<float> X { get; private set; }
		public FCurve<float> Y { get; private set; }
		public FCurve<float> Z { get; private set; }

		public FCurveVector3D()
		{
			X = new FCurve<float>(0);
			Y = new FCurve<float>(0);
			Z = new FCurve<float>(0);
		}

		public byte[] GetBytes(float mul = 1.0f)
		{
			List<byte[]> data = new List<byte[]>();
			data.Add(X.GetBytes(mul));
			data.Add(Y.GetBytes(mul));
			data.Add(Z.GetBytes(mul));
			return data.SelectMany(_ => _).ToArray();
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class Float
	{
		float _value = 0;
		float _max = float.MaxValue;
		float _min = float.MinValue;
		
		public float Value
		{
			get
			{
				return GetValue();
			}

			set
			{
				SetValue(value);
			}
		}

		/// <summary>
		/// 変更単位量
		/// </summary>
		public float Step
		{
			get;
			set;
		}

		public float RangeMin
		{
			get { return _min; }
		}

		public float RangeMax
		{
			get { return _max; }
		}

		public float DefaultValue { get; private set; }

		public bool IsValueChangedFromDefault
		{
			get { return Value != DefaultValue; }
		}

		public event ChangedValueEventHandler OnChanged;

		internal Float(float value = 0, float max = float.MaxValue, float min = float.MinValue, float step = 1.0f)
		{
			_max = max;
			_min = min;
			_value = value.Clipping(_max, _min);
			Step = step;
			DefaultValue = _value;
		}

		protected void CallChanged(object value, ChangedValueType type)
		{
			if (OnChanged != null)
			{
				OnChanged(this, new ChangedValueEventArgs(value, type));
			}
		}

		public float GetValue()
		{
			return _value;
		}

		public void SetValue(float value, bool isCombined = false)
		{
			value = value.Clipping(_max, _min);

			if (value == _value) return;

			float old_value = _value;
			float new_value = value;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_value = new_value;

					CallChanged(new_value, ChangedValueType.Execute);
				},
				() =>
				{
					_value = old_value;

					CallChanged(old_value, ChangedValueType.Unexecute);
				},
				this,
				isCombined);

			Command.CommandManager.Execute(cmd);
		}

		public void SetValueDirectly(float value)
		{
			var converted = value.Clipping(_max, _min);
			if (_value == converted) return;
			_value = converted;

			CallChanged(_value, ChangedValueType.Execute);
		}

		public static implicit operator float(Float value)
		{
			return value._value;
		}

		public static explicit operator byte[](Float value)
		{
			byte[] values = new byte[sizeof(float) * 1];
			BitConverter.GetBytes(value.Value).CopyTo(values, sizeof(float) * 0);
			return values;
		}

		public byte[] GetBytes(float mul = 1.0f)
		{
			byte[] values = new byte[sizeof(float) * 1];
			BitConverter.GetBytes(Value * mul).CopyTo(values, sizeof(float) * 0);
			return values;
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class FloatWithRandom
	{
		float _value_center = 0;
		float _value_max = 0;
		float _value_min = 0;
		
		float _max = float.MaxValue;
		float _min = float.MinValue;
		
		public float Center
		{
			get
			{
				return GetCenter();
			}

			set
			{
				SetCenter(value);
			}
		}

		public float Max
		{
			get
			{
				return GetMax();
			}

			set
			{
				SetMax(value);
			}
		}

		public float Min
		{
			get
			{
				return GetMin();
			}

			set
			{
				SetMin(value);
			}
		}

		public float Amplitude
		{
			get
			{
				return GetAmplitude();
			}

			set
			{
				SetAmplitude(value);
			}
		}

		public DrawnAs DrawnAs
		{
			get;
			set;
		}

		public float Step
		{
			get;
			set;
		}

		public float ValueMin
		{
			get { return _min; }
		}

		public float ValueMax
		{
			get { return _max; }
		}

		public event ChangedValueEventHandler OnChanged;

		internal float DefaultValueCenter { get; private set; }
		internal float DefaultValueMax { get; private set; }
		internal float DefaultValueMin { get; private set; }
		internal DrawnAs DefaultDrawnAs { get; private set; }

		public bool IsValueChangedFromDefault
		{
			get { return Center != DefaultValueCenter || Min != DefaultValueMin || Max != DefaultValueMax || DrawnAs != DefaultDrawnAs; }
		}

		internal FloatWithRandom(float value = 0, float max = float.MaxValue, float min = float.MinValue, DrawnAs drawnas = Data.DrawnAs.CenterAndAmplitude, float step = 1.0f)
		{
			_max = max;
			_min = min;
			_value_center = value.Clipping(_max, _min);
			_value_max = value.Clipping(_max, _min);
			_value_min = value.Clipping(_max, _min);
			
			DrawnAs = drawnas;
			Step = step;

			DefaultValueCenter = _value_center;
			DefaultValueMax = _value_max;
			DefaultValueMin = _value_min;
			DefaultDrawnAs = DrawnAs;
		}

		protected void CallChanged(object value, ChangedValueType type)
		{
			if (OnChanged != null)
			{
				OnChanged(this, new ChangedValueEventArgs(value, type));
			}
		}

		public float GetCenter()
		{
			return _value_center;
		}

		public void SetCenter(float value, bool isCombined = false)
		{
			value = Math.Min(value, _max);
			value = Math.Max(value, _min);

			if (value == _value_center) return;

			float a = Amplitude;

			float old_center = _value_center;
			float new_center = value;
			float old_max = _value_max;
			float new_max = Math.Min(value + a, _max );
			float old_min = _value_min;
			float new_min = Math.Max(value - a, _min);

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_value_center = new_center;
					_value_max = new_max;
					_value_min = new_min;

					CallChanged(new_center, ChangedValueType.Execute);
				},
				() =>
				{
					_value_center = old_center;
					_value_max = old_max;
					_value_min = old_min;

					CallChanged(old_center, ChangedValueType.Unexecute);
				},
				this,
				isCombined);

			Command.CommandManager.Execute(cmd);
		}

		public void SetCenterDirectly(float value)
		{
			_value_center = value;
		}

		public float GetMax()
		{
			return _value_max;
		}

		public void SetMax(float value, bool isCombined = false)
		{
			value = Math.Min(value, _max);
			value = Math.Max(value, _min);

			if (value == _value_max) return;

			float old_max = _value_max;
			float new_max = value;
			float old_min = _value_min;
			float new_min = Math.Min(value, _value_min);
			float old_center = _value_center;
			float new_center = (new_max + new_min) / 2.0f;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_value_center = new_center;
					_value_max = new_max;
					_value_min = new_min;

					CallChanged(new_max, ChangedValueType.Execute);
				},
				() =>
				{
					_value_center = old_center;
					_value_max = old_max;
					_value_min = old_min;

					CallChanged(old_max, ChangedValueType.Unexecute);
				},
				this,
				isCombined);

			Command.CommandManager.Execute(cmd);
		}

		public void SetMaxDirectly(float value)
		{
			_value_max = value;
		}

		public float GetMin()
		{
			return _value_min;
		}

		public void SetMin(float value, bool isCombined = false)
		{
			value = Math.Min(value, _max);
			value = Math.Max(value, _min);

			if (value == _value_min) return;

			float old_max = _value_max;
			float new_max = Math.Max(value, _value_max);
			float old_min = _value_min;
			float new_min = value;
			float old_center = _value_center;
			float new_center = (new_max + new_min) / 2.0f;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_value_center = new_center;
					_value_max = new_max;
					_value_min = new_min;

					CallChanged(new_min, ChangedValueType.Execute);
				},
				() =>
				{
					_value_center = old_center;
					_value_max = old_max;
					_value_min = old_min;

					CallChanged(old_min, ChangedValueType.Unexecute);
				},
				this,
				isCombined);

			Command.CommandManager.Execute(cmd);
		}

		public void SetMinDirectly(float value)
		{
			_value_min = value;
		}

		public float GetAmplitude()
		{
			return Math.Max(_value_max - _value_center, _value_center - _value_min);
		}

		public void SetAmplitude(float value, bool isCombined = false)
		{
			value = Math.Max(value, 0);

			float old_center = _value_center;
			float new_center = _value_center;
			float old_max = _value_max;
			float new_max = Math.Min(_value_center + value, _max);
			float old_min = _value_min;
			float new_min = Math.Max(_value_center - value, _min);

			if (new_max == old_max && new_min == old_min) return;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_value_center = new_center;
					_value_max = new_max;
					_value_min = new_min;

					CallChanged(new_min, ChangedValueType.Execute);
				},
				() =>
				{
					_value_center = old_center;
					_value_max = old_max;
					_value_min = old_min;

					CallChanged(old_min, ChangedValueType.Unexecute);
				},
				this,
				isCombined);

			Command.CommandManager.Execute(cmd);
		}

		public void SetAmplitudeDirectly(float value)
		{
			float old_center = _value_center;
			float new_center = _value_center;
			float old_max = _value_max;
			float new_max = Math.Min(_value_center + value, _max);
			float old_min = _value_min;
			float new_min = Math.Max(_value_center - value, _min);
			_value_center = new_center;
			_value_max = new_max;
			_value_min = new_min;
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class Int
	{
		int _value = 0;
		int _max = int.MaxValue;
		int _min = int.MinValue;
		
		public int Min
		{
			get
			{
				return _min;
			}
		}

		public int Max
		{
			get
			{
				return _max;
			}
		}

		public int Value
		{
			get
			{
				return GetValue();
			}

			set
			{
				SetValue(value);
			}
		}
		
		/// <summary>
		/// 変更単位量
		/// </summary>
		public int Step
		{
			get;
			set;
		}

		public event ChangedValueEventHandler OnChanged;

		public int DefaultValue { get; private set; }

		public bool IsValueChangedFromDefault
		{
			get { return Value != DefaultValue; }
		}

		internal Int(int value = 0, int max = int.MaxValue, int min = int.MinValue, int step = 1 )
		{
			_max = max;
			_min = min;
			_value = value.Clipping(_max, _min);
			Step = step;

			DefaultValue = value;
		}

		protected void CallChanged(object value, ChangedValueType type)
		{
			if (OnChanged != null)
			{
				OnChanged(this, new ChangedValueEventArgs(value, type));
			}
		}

		public int GetValue()
		{
			return _value;
		}

		public void SetValue(int value, bool isCombined = false)
		{
			value = value.Clipping(_max, _min);

			if (value == _value) return;

			int old_value = _value;
			int new_value = value;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_value = new_value;

					CallChanged(new_value, ChangedValueType.Execute);
				},
				() =>
				{
					_value = old_value;

					CallChanged(old_value, ChangedValueType.Unexecute);
				},
				this,
				isCombined);

			Command.CommandManager.Execute(cmd);
		}

		public void SetValueDirectly(int value)
		{
			var converted = value.Clipping(_max, _min);
			if (_value == converted) return;
			_value = converted;

			CallChanged(_value, ChangedValueType.Execute);
		}

		public static implicit operator int(Int value)
		{
			return value._value;
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class IntWithInifinite
	{
		public Int Value
		{
			get;
			private set;
		}
		public Boolean Infinite
		{
			get;
			private set;
		}
		public int Step
		{
			get;
			private set;
		}

		public bool IsValueChangedFromDefault
		{
			get { return Value.IsValueChangedFromDefault || Infinite.IsValueChangedFromDefault; }
		}

		internal IntWithInifinite(int value = 0, bool infinite = false, int max = int.MaxValue, int min = int.MinValue, int step = 1)
		{
			Value = new Int(value, max, min, step);
			Infinite = new Boolean(infinite);
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class IntWithRandom
	{
		int _value_center = 0;
		int _value_max = 0;
		int _value_min = 0;
		
		int _max = int.MaxValue;
		int _min = int.MinValue;
		
		public int Center
		{
			get
			{
				return GetCenter();
			}

			set
			{
				SetCenter(value);
			}
		}

		public int Max
		{
			get
			{
				return GetMax();
			}

			set
			{
				SetMax(value);
			}
		}

		public int Min
		{
			get
			{
				return GetMin();
			}

			set
			{
				SetMin(value);
			}
		}

		public int Amplitude
		{
			get
			{
				return GetAmplitude();
			}

			set
			{
				SetAmplitude(value);
			}
		}

		public DrawnAs DrawnAs
		{
			get;
			set;
		}

		public int Step
		{
			get;
			set;
		}

		public int ValueMin
		{
			get { return _min; }
		}

		public int ValueMax
		{
			get { return _max; }
		}

		internal int DefaultValueCenter { get; private set; }
		internal int DefaultValueMax { get; private set; }
		internal int DefaultValueMin { get; private set; }
		internal DrawnAs DefaultDrawnAs { get; private set; }

		public event ChangedValueEventHandler OnChanged;

		public bool IsValueChangedFromDefault
		{
			get { return Center != DefaultValueCenter || Min != DefaultValueMin || Max != DefaultValueMax || DrawnAs != DefaultDrawnAs; }
		}

		internal IntWithRandom(int value = 0, int max = int.MaxValue, int min = int.MinValue, DrawnAs drawnas = Data.DrawnAs.CenterAndAmplitude, int step = 1)
		{
			_value_center = value;
			_value_max = value;
			_value_min = value;
			_max = max;
			_min = min;
			DrawnAs = drawnas;
			Step = step;

			DefaultValueCenter = _value_center;
			DefaultValueMax = _value_max;
			DefaultValueMin = _value_min;
			DefaultDrawnAs = DrawnAs;
		}

		internal void CallChanged(object value, ChangedValueType type)
		{
			if (OnChanged != null)
			{
				OnChanged(this, new ChangedValueEventArgs(value, type));
			}
		}

		public int GetCenter()
		{
			return _value_center;
		}

		public void SetCenter(int value, bool isCombined = false)
		{
			value = Math.Min(value, _max);
			value = Math.Max(value, _min);

			if (value == _value_center) return;

			int a = Amplitude;

			int old_center = _value_center;
			int new_center = value;
			int old_max = _value_max;
			int new_max = Math.Min(value + a, _max );
			int old_min = _value_min;
			int new_min = Math.Max(value - a, _min);

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_value_center = new_center;
					_value_max = new_max;
					_value_min = new_min;

					CallChanged(new_center, ChangedValueType.Execute);
				},
				() =>
				{
					_value_center = old_center;
					_value_max = old_max;
					_value_min = old_min;

					CallChanged(old_center, ChangedValueType.Unexecute);
				},
				this,
				isCombined);

			Command.CommandManager.Execute(cmd);
		}

		internal void SetCenterDirectly(int value)
		{
			_value_center = value;
		}

		public int GetMax()
		{
			return _value_max;
		}

		public void SetMax(int value, bool isCombined = false)
		{
			value = Math.Min(value, _max);
			value = Math.Max(value, _min);

			if (value == _value_max) return;

			int old_max = _value_max;
			int new_max = value;
			int old_min = _value_min;
			int new_min = Math.Min(value, _value_min);
			int old_center = _value_center;
			int new_center = (new_max + new_min) / 2;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_value_center = new_center;
					_value_max = new_max;
					_value_min = new_min;

					CallChanged(new_max, ChangedValueType.Execute);
				},
				() =>
				{
					_value_center = old_center;
					_value_max = old_max;
					_value_min = old_min;

					CallChanged(old_max, ChangedValueType.Unexecute);
				},
				this,
				isCombined);

			Command.CommandManager.Execute(cmd);
		}

		internal void SetMaxDirectly(int value)
		{
			_value_max = value;
		}

		public int GetMin()
		{
			return _value_min;
		}

		public void SetMin(int value, bool isCombined = false)
		{
			value = Math.Min(value, _max);
			value = Math.Max(value, _min);

			if (value == _value_min) return;

			int old_max = _value_max;
			int new_max = Math.Max(value, _value_max);
			int old_min = _value_min;
			int new_min = value;
			int old_center = _value_center;
			int new_center = (new_max + new_min) / 2;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_value_center = new_center;
					_value_max = new_max;
					_value_min = new_min;

					CallChanged(new_min, ChangedValueType.Execute);
				},
				() =>
				{
					_value_center = old_center;
					_value_max = old_max;
					_value_min = old_min;

					CallChanged(old_min, ChangedValueType.Unexecute);
				},
				this,
				isCombined);

			Command.CommandManager.Execute(cmd);
		}

		internal void SetMinDirectly(int value)
		{
			_value_min = value;
		}

		public int GetAmplitude()
		{
			return Math.Max(_value_max - _value_center, _value_center - _value_min);
		}

		public void SetAmplitude(int value, bool isCombined = false)
		{
			value = Math.Max(value, 0);

			int old_center = _value_center;
			int new_center = _value_center;
			int old_max = _value_max;
			int new_max = Math.Min(_value_center + value, _max);
			int old_min = _value_min;
			int new_min = Math.Max(_value_center - value, _min);

			if (new_max == old_max && new_min == old_min) return;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_value_center = new_center;
					_value_max = new_max;
					_value_min = new_min;

					CallChanged(new_min, ChangedValueType.Execute);
				},
				() =>
				{
					_value_center = old_center;
					_value_max = old_max;
					_value_min = old_min;

					CallChanged(old_min, ChangedValueType.Unexecute);
				},
				this,
				isCombined);

			Command.CommandManager.Execute(cmd);
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class Path
	{
		string _abspath = string.Empty;

		/// <summary>
		/// 相対パスで保存されるか?
		/// </summary>
		public bool IsRelativeSaved
		{
			get;
			private set;
		}

		public string Filter
		{
			get;
			private set;
		}

		public string AbsolutePath
		{
			get
			{
				return GetAbsolutePath();
			}

			set
			{
				SetAbsolutePath(value);
			}
		}

		public string RelativePath
		{
			get
			{
				return GetRelativePath();
			}
			set
			{
				SetRelativePath(value);
			}
		}

		public event ChangedValueEventHandler OnChanged;

		public string DefaultValue { get; private set; }

		internal Path(string filter, bool isRelativeSaved = true, string abspath = "")
		{
			Filter = filter;
			IsRelativeSaved = isRelativeSaved;
			_abspath = abspath;

			DefaultValue = _abspath;
		}

		public string GetAbsolutePath()
		{
			return _abspath;
		}

		public void SetAbsolutePath(string abspath)
		{
			if (abspath == _abspath) return;

			var old_value = _abspath;
			var new_value = abspath;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_abspath = new_value;

					if (OnChanged != null)
					{
						OnChanged(this, new ChangedValueEventArgs(new_value, ChangedValueType.Execute));
					}
				},
				() =>
				{
					_abspath = old_value;

					if (OnChanged != null)
					{
						OnChanged(this, new ChangedValueEventArgs(old_value, ChangedValueType.Unexecute));
					}
				});

			Command.CommandManager.Execute(cmd);
		}

		public string GetRelativePath()
		{
			if (_abspath == string.Empty) return _abspath;
			if (Core.FullPath == string.Empty) return _abspath;

			Uri basepath = new Uri(Core.FullPath);
			Uri path = new Uri(_abspath);
			var relative_path = basepath.MakeRelativeUri(path).ToString();

#if ESCAPE_URI_ENABLED
            relative_path = System.Web.HttpUtility.UrlDecode(relative_path);
#endif
            return relative_path;
		}

		public void SetRelativePath(string relative_path)
		{
			try
			{
				if (Core.FullPath == string.Empty)
				{
					SetAbsolutePath(relative_path);
					return;
				}

				if (relative_path == string.Empty)
				{
					SetAbsolutePath(string.Empty);
				}
				else
				{
					Uri basepath = new Uri(Core.FullPath);
					Uri path = new Uri(basepath, relative_path);
					var absolute_path = path.LocalPath;
					SetAbsolutePath(absolute_path);
				}
			}
			catch (Exception e)
			{
				throw new Exception(e.ToString() + " Core.FullPath = " + Core.FullPath );
			}
		}

		internal void SetAbsolutePathDirectly(string abspath)
		{
			if (_abspath == abspath) return;

			_abspath = abspath;
			if (OnChanged != null)
			{
				OnChanged(this, new ChangedValueEventArgs(_abspath, ChangedValueType.Execute));
			}
		}

		internal void SetRelativePathDirectly(string relative_path)
		{
			if (Core.FullPath == string.Empty) SetAbsolutePathDirectly(relative_path);
			Uri basepath = new Uri(Core.FullPath);
			Uri path = new Uri(basepath, relative_path);
			var absolute_path = path.LocalPath;
			SetAbsolutePathDirectly(absolute_path);
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class PathForImage : Path
	{
		internal PathForImage(string filter, bool isRelativeSaved, string abspath = "")
			: base(filter, isRelativeSaved, abspath)
		{
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class PathForModel : Path
	{
		internal PathForModel(string filter, bool isRelativeSaved, string abspath = "")
			: base(filter, isRelativeSaved, abspath)
		{
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class PathForSound : Path
	{
		internal PathForSound(string filter, bool isRelativeSaved, string abspath = "")
			: base(filter, isRelativeSaved, abspath)
		{
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class String
	{
		string _value = string.Empty;

		public string Value
		{
			get
			{
				return GetValue();
			}

			set
			{
				SetValue(value);
			}
		}

		public event ChangedValueEventHandler OnChanged;

		public string DefaultValue { get; private set; }

		public bool IsValueChangedFromDefault
		{
			get { return Value != DefaultValue; }
		}

		internal String(string value = "")
		{
			_value = value;
			DefaultValue = _value;
		}

		public string GetValue()
		{
			return _value;
		}

		public void SetValue(string value)
		{
			if (value == _value) return;

			string old_value = _value;
			string new_value = value;

			var cmd = new Command.DelegateCommand(
				() =>
				{
					_value = new_value;

					if (OnChanged != null)
					{
						OnChanged(this, new ChangedValueEventArgs(new_value, ChangedValueType.Execute));
					}
				},
				() =>
				{
					_value = old_value;

					if (OnChanged != null)
					{
						OnChanged(this, new ChangedValueEventArgs(old_value, ChangedValueType.Unexecute));
					}
				});

			Command.CommandManager.Execute(cmd);
		}

		internal void SetValueDirectly(string value)
		{
			if (_value == value) return;

			_value = value;
			if (OnChanged != null)
			{
				OnChanged(this, new ChangedValueEventArgs(_value, ChangedValueType.Execute));
			}
		}

		public static implicit operator string(String value)
		{
			return value._value;
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class Vector2D
	{
		public Float X
		{
			get;
			private set;
		}

		public Float Y
		{
			get;
			private set;
		}

		public bool IsValueChangedFromDefault
		{
			get { return X.IsValueChangedFromDefault || Y.IsValueChangedFromDefault; }
		}

		internal Vector2D(
			float x = 0,
			float y = 0,
			float x_max = float.MaxValue,
			float x_min = float.MinValue,
			float y_max = float.MaxValue,
			float y_min = float.MinValue,
			float x_step = 1.0f,
			float y_step = 1.0f)
		{
			X = new Float(x, x_max, x_min, x_step);
			Y = new Float(y, y_max, y_min, y_step);
		}

		public static explicit operator byte[](Vector2D value)
		{
			byte[] values = new byte[sizeof(float) * 2];
			byte[] x = BitConverter.GetBytes(value.X.GetValue());
			byte[] y = BitConverter.GetBytes(value.Y.GetValue());
			x.CopyTo(values, 0);
			y.CopyTo(values, sizeof(float) * 1);
			return values;
		}

		public byte[] GetBytes(float mul = 1.0f)
		{
			byte[] values = new byte[sizeof(float) * 2];
			BitConverter.GetBytes(X * mul).CopyTo(values, sizeof(float) * 0);
			BitConverter.GetBytes(Y * mul).CopyTo(values, sizeof(float) * 1);
			return values;
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class Vector2DWithRandom
	{
		public FloatWithRandom X
		{
			get;
			private set;
		}

		public FloatWithRandom Y
		{
			get;
			private set;
		}

		public DrawnAs DrawnAs
		{
			get;
			set;
		}

		internal DrawnAs DefaultDrawnAs { get; private set; }

		public bool IsValueChangedFromDefault
		{
			get { return X.IsValueChangedFromDefault || Y.IsValueChangedFromDefault || DrawnAs != DefaultDrawnAs; }
		}

		internal Vector2DWithRandom(
			float x = 0,
			float y = 0,
			float x_max = float.MaxValue,
			float x_min = float.MinValue,
			float y_max = float.MaxValue,
			float y_min = float.MinValue,
			DrawnAs drawnas = Data.DrawnAs.CenterAndAmplitude,
			float x_step = 1.0f,
			float y_step = 1.0f)
		{
			X = new FloatWithRandom(x, x_max, x_min, drawnas, x_step);
			Y = new FloatWithRandom(y, y_max, y_min, drawnas, y_step);
			DrawnAs = drawnas;
			DefaultDrawnAs = DrawnAs;
        }

        public byte[] GetBytes(float mul = 1.0f)
        {
            byte[] values = new byte[sizeof(float) * 4];
            BitConverter.GetBytes(X.Max * mul).CopyTo(values, sizeof(float) * 0);
            BitConverter.GetBytes(Y.Max * mul).CopyTo(values, sizeof(float) * 1);
            BitConverter.GetBytes(X.Min * mul).CopyTo(values, sizeof(float) * 2);
            BitConverter.GetBytes(Y.Min * mul).CopyTo(values, sizeof(float) * 3);
            return values;
        }

        public static explicit operator byte[](Vector2DWithRandom value)
        {
            return value.GetBytes();
        }
	}
}

namespace EffekseerTool.Data.Value
{
	public class Vector3D
	{
		public Float X
		{
			get;
			private set;
		}

		public Float Y
		{
			get;
			private set;
		}

		public Float Z
		{
			get;
			private set;
		}

		public bool IsValueChangedFromDefault
		{
			get { return X.IsValueChangedFromDefault || Y.IsValueChangedFromDefault || Z.IsValueChangedFromDefault; }
		}


		internal Vector3D(
			float x = 0,
			float y = 0,
			float z = 0,
			float x_max = float.MaxValue,
			float x_min = float.MinValue,
			float y_max = float.MaxValue,
			float y_min = float.MinValue,
			float z_max = float.MaxValue,
			float z_min = float.MinValue,
			float x_step = 1.0f,
			float y_step = 1.0f,
			float z_step = 1.0f)
		{
			X = new Float(x, x_max, x_min, x_step);
			Y = new Float(y, y_max, y_min, y_step);
			Z = new Float(z, z_max, z_min, z_step);
		}

		public static explicit operator byte[](Vector3D value)
		{
			byte[] values = new byte[sizeof(float) * 3];
			byte[] x = BitConverter.GetBytes(value.X.GetValue());
			byte[] y = BitConverter.GetBytes(value.Y.GetValue());
			byte[] z = BitConverter.GetBytes(value.Z.GetValue());
			x.CopyTo(values, 0);
			y.CopyTo(values, sizeof(float) * 1);
			z.CopyTo(values, sizeof(float) * 2);
			return values;
		}
	}
}

namespace EffekseerTool.Data.Value
{
	public class Vector3DWithRandom
	{
		public FloatWithRandom X
		{
			get;
			private set;
		}

		public FloatWithRandom Y
		{
			get;
			private set;
		}

		public FloatWithRandom Z
		{
			get;
			private set;
		}

		public DrawnAs DrawnAs
		{
			get;
			set;
		}

		internal DrawnAs DefaultDrawnAs { get; private set; }

		public bool IsValueChangedFromDefault
		{
			get { return X.IsValueChangedFromDefault || Y.IsValueChangedFromDefault || Z.IsValueChangedFromDefault || DrawnAs != DefaultDrawnAs; }
		}

		internal Vector3DWithRandom(
			float x = 0,
			float y = 0,
			float z = 0,
			float x_max = float.MaxValue,
			float x_min = float.MinValue,
			float y_max = float.MaxValue,
			float y_min = float.MinValue,
			float z_max = float.MaxValue,
			float z_min = float.MinValue,
			DrawnAs drawnas = Data.DrawnAs.CenterAndAmplitude,
			float x_step = 1.0f,
			float y_step = 1.0f,
			float z_step = 1.0f)
		{
			X = new FloatWithRandom(x, x_max, x_min, drawnas, x_step);
			Y = new FloatWithRandom(y, y_max, y_min, drawnas, y_step);
			Z = new FloatWithRandom(z, z_max, z_min, drawnas, z_step);
			DrawnAs = drawnas;
			DefaultDrawnAs = DrawnAs;
		}

		public byte[] GetBytes( float mul = 1.0f)
		{
			byte[] values = new byte[sizeof(float) * 6];
			BitConverter.GetBytes(X.Max * mul).CopyTo(values, sizeof(float) * 0);
			BitConverter.GetBytes(Y.Max * mul).CopyTo(values, sizeof(float) * 1);
			BitConverter.GetBytes(Z.Max * mul).CopyTo(values, sizeof(float) * 2);
			BitConverter.GetBytes(X.Min * mul).CopyTo(values, sizeof(float) * 3);
			BitConverter.GetBytes(Y.Min * mul).CopyTo(values, sizeof(float) * 4);
			BitConverter.GetBytes(Z.Min * mul).CopyTo(values, sizeof(float) * 5);
			return values;
		}

		public static explicit operator byte[](Vector3DWithRandom value)
		{
			return value.GetBytes();
		}
	}
}

namespace EffekseerTool.Utl
{
	public static class ArrayExtensions
	{
		public static T[] ToArray<T>(this T[][] arrayArray)
		{
			int count = 0;
			T[] ret = null;

			foreach (var ar in arrayArray)
			{
				count += ar.Length;
			}

			ret = new T[count];

			int offset = 0;
			foreach (var ar in arrayArray)
			{
				foreach (var a in ar)
				{
					ret[offset] = a;
					offset++;
				}
			}

			return ret;
		}

		public static T[] ToArray<T>(this List<T[]> arrayList)
		{
			return arrayList.ToArray().ToArray();
		}
	}
}

namespace EffekseerTool.Utl
{
	public static class BitConverterExtensions
	{
		public static byte[] GetBytes(this int value)
		{
			return BitConverter.GetBytes(value);
		}

		public static byte[] GetBytes<T>(this T obj) where T : struct
		{
			int size = Marshal.SizeOf(obj);
			byte[] bytes = new byte[size];
			GCHandle gch = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			Marshal.StructureToPtr(obj, gch.AddrOfPinnedObject(), false);
			gch.Free();
			return bytes;
		}
	}
}

namespace EffekseerTool.Utl
{
	class MathUtl
	{
		/// <summary>
		/// Easingの三次方程式の項3つを求める。
		/// </summary>
		/// <param name="a1">始点の角度(度)</param>
		/// <param name="a2">終点の角度(度)</param>
		/// <returns></returns>
		public static float[] Easing(float a1, float a2)
		{ 
			float g1 = (float)Math.Tan( ((float)a1 + 45.0) / 180.0 * Math.PI );
			float g2 = (float)Math.Tan( ((float)a2 + 45.0) / 180.0 * Math.PI );

			float c = g1;
			float a = g2 - g1 - ( 1.0f - c ) * 2.0f;
			float b = (g2 - g1 - (a * 3.0f)) / 2.0f;

			return new float[3] { a, b, c };
		}
	}
}

namespace EffekseerTool.Utl
{
	public static class MathExtensions
	{
		public static int Clipping(this int v, int max, int min)
		{
			if (v > max) return max;
			if (v < min) return min;
			return v;
		}

		public static float Clipping(this float v, float max, float min)
		{
			if (v > max) return max;
			if (v < min) return min;
			return v;
		}

		public static double Clipping(this double v, double max, double min)
		{
			if (v > max) return max;
			if (v < min) return min;
			return v;
		}
	}
}

namespace EffekseerTool.Utl
{
	/// <summary>
	/// Model format
	/// </summary>
	/// <remarks>
	/// Version 0 First version
	/// Version 1 Vertex color is added
	/// Version 2 Scale is added into header (this version is only 1.30 beta2)
	/// Version 3 Scale is added into footer (compatible with Version1)
	/// </remarks>
	public class ModelInformation
	{
		public float Scale = 1.0f;

		public ModelInformation()
		{

		}

		public bool Load(string path)
		{
			System.IO.FileStream fs = null;
			try
			{
				fs = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
			}
			catch (System.IO.FileNotFoundException e)
			{
				return false;
			}

			var br = new System.IO.BinaryReader(fs);

			var buf = new byte[1024];

			if (br.Read(buf, 0, 8) != 8)
			{
				fs.Dispose();
                br.Close();
                return false;
			}

			var version = BitConverter.ToInt32(buf, 0);

			if (version == 2)
			{
				Scale = BitConverter.ToSingle(buf, 4);
				fs.Dispose();
                br.Close();
                return false;
			}

			if(version == 3)
			{
				fs.Seek(-4, System.IO.SeekOrigin.End);

				if (br.Read(buf, 0, 4) == 4)
				{
					Scale = BitConverter.ToSingle(buf, 0);
				}
				else
				{
					fs.Dispose();
                    br.Close();
                    return false;
				}
			}

			if (version == 5)
			{
				Scale = BitConverter.ToSingle(buf, 4);
				fs.Dispose();
                br.Close();
                return true;
			}

			fs.Dispose();
            br.Close();

            return true;
		}
	}
}

namespace EffekseerTool.Utl
{
	/// <summary>
	/// 有効なパラメーター一覧を取得するためのツリーノード
	/// </summary>
	public class ParameterTreeNode
	{
		public Data.NodeBase Node { get; private set; }

		public Tuple35<string,object>[] Parameters { get; private set; }

		public ParameterTreeNode[] Children { get; private set; }

		public ParameterTreeNode(Data.NodeBase node, Tuple35<string, object>[] paramaters, ParameterTreeNode[] children)
		{
			Node = node;
			Parameters = paramaters;
			Children = children;
		}
	}
}

namespace EffekseerTool.Utl
{
	public class TextureInformation
	{
		public int Width { get; private set; }
		public int Height { get; private set; }

		public TextureInformation()
		{ 
		
		}

		public bool Load(string path)
		{ 

			System.IO.FileStream fs = null;
            if (!System.IO.File.Exists(path)) return false;

            try
			{
				fs = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
			}
			catch(System.IO.FileNotFoundException e)
			{
				return false;
			}

			var br = new System.IO.BinaryReader(fs);

			var buf = new byte[1024];

			if (br.Read(buf, 0, 8) != 8)
			{
				fs.Dispose();
				br.Close();
				return false;
			}

			// png Header 89 50 4E 47 0D 0A 1A 0A
			if (buf[0] == 0x89 &&
				buf[1] == 0x50 &&
				buf[2] == 0x4E &&
				buf[3] == 0x47 &&
				buf[4] == 0x0D &&
				buf[5] == 0x0A &&
				buf[6] == 0x1A &&
				buf[7] == 0x0A)
			{
				// PNG
				if (br.Read(buf, 0, 25) != 25)
				{
					fs.Dispose();
                    br.Close();
                    return false;
				}

				var width = new byte[] { buf[11], buf[10], buf[9], buf[8] };
				var height = new byte[] { buf[15], buf[14], buf[13], buf[12] };
				Width = BitConverter.ToInt32(width, 0);
				Height = BitConverter.ToInt32(height, 0);
			}
			else if (buf[0] == 0x44 && buf[1] == 0x44 && buf[2] == 0x53)
			{
				// DDS
				if (br.Read(buf, 0, 25) != 25)
				{
					fs.Dispose();
                    br.Close();
                    return false;
				}

				Width = BitConverter.ToInt32(buf, 8);
				Height = BitConverter.ToInt32(buf, 4);
			}
			else
			{
				fs.Dispose();
                br.Close();
                return false;
			}

			fs.Dispose();
            br.Close();
            return true;
		}
	}
}

namespace EffekseerTool.Utl
{
	public static class XmlDocumentExtensions
	{
		/// <summary>
		/// ChildNodesがXmlTextのみ1つの時、そのXmlTextの値を取得する。
		/// </summary>
		/// <param name="xmlnode"></param>
		/// <returns></returns>
		public static string GetText(this System.Xml.XmlNode xmlnode)
		{
			if (xmlnode.ChildNodes.Count == 1 && xmlnode.ChildNodes[0] is System.Xml.XmlText)
			{
				return xmlnode.ChildNodes[0].Value;
			}

			return string.Empty;
		}

		public static int GetTextAsInt(this System.Xml.XmlNode xmlnode)
		{
			int ret = 0;

			if (xmlnode.ChildNodes.Count == 1 && xmlnode.ChildNodes[0] is System.Xml.XmlText)
			{
				int.TryParse(xmlnode.ChildNodes[0].Value, System.Globalization.NumberStyles.Integer, Setting.NFI, out ret);
			}

			return ret;
		}

		public static float GetTextAsFloat(this System.Xml.XmlNode xmlnode)
		{
			float ret = 0;

			if (xmlnode.ChildNodes.Count == 1 && xmlnode.ChildNodes[0] is System.Xml.XmlText)
			{
				float.TryParse(xmlnode.ChildNodes[0].Value, System.Globalization.NumberStyles.Float, Setting.NFI, out ret);
			}

			return ret;
		}

		/// <summary>
		/// ChildNodesからlocalnameのXmlNodeを探し出し、そのXmlNodeのChildNodesがXmlTextのみ1つの時、そのXmlTextの値を取得する。
		/// </summary>
		/// <param name="xmlnode"></param>
		/// <param name="localname"></param>
		/// <returns></returns>
		public static string GetText(this System.Xml.XmlNode xmlnode, string localname)
		{
			var local = xmlnode[localname];

			if (local == null) return string.Empty;

			return GetText(local);
		}

		public static int GetTextAsInt(this System.Xml.XmlNode xmlnode, string localname)
		{
			var local = xmlnode[localname];

			if (local == null) return 0;

			return GetTextAsInt(local);
		}

		public static float GetTextAsFloat(this System.Xml.XmlNode xmlnode, string localname)
		{
			var local = xmlnode[localname];

			if (local == null) return 0;

			return GetTextAsFloat(local);
		}

		public static System.Xml.XmlElement[] GetElements(this System.Xml.XmlNode xmlnode, string localname)
		{
			List<System.Xml.XmlElement> elements = new List<System.Xml.XmlElement>();

			for (int i = 0; i < xmlnode.ChildNodes.Count; i++)
			{
				if (xmlnode.ChildNodes[i].LocalName == localname && xmlnode.ChildNodes[i] is System.Xml.XmlElement)
				{
					elements.Add((System.Xml.XmlElement)xmlnode.ChildNodes[i]);
				}
			}

			return elements.ToArray();
		}

		public static bool TryGet(this System.Xml.XmlNode xmlnode, string localname, ref string ret)
		{
			var local = xmlnode[localname];

			if (local == null) return false;

			ret = GetText(local);

			return true;
		}

		public static bool TryGet(this System.Xml.XmlNode xmlnode, string localname, ref int ret)
		{
			var local = xmlnode[localname];

			if (local == null) return false;

			ret = GetTextAsInt(local);

			return true;
		}

		public static bool TryGet(this System.Xml.XmlNode xmlnode, string localname, ref float ret)
		{
			var local = xmlnode[localname];

			if (local == null) return false;

			ret = GetTextAsFloat(local);

			return true;
		}

		public static System.Xml.XmlElement CreateTextElement(this System.Xml.XmlDocument doc, string name, string text)
		{
			var element = doc.CreateElement(name);
			element.AppendChild(doc.CreateTextNode(text));
			return element;
		}

		public static System.Xml.XmlElement CreateTextElement(this System.Xml.XmlDocument doc, string name, int value)
		{
			return CreateTextElement(doc, name, value.ToString());
		}
	}
}
#pragma warning restore
#endif
