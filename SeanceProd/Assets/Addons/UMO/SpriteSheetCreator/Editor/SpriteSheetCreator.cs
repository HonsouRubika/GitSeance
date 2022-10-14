/// Unity Modules - Sprite Sheet Creator
/// Created by: Nicolas Capelier
/// Contact: capelier.nicolas@gmail.com
/// Version: 1.0.1
/// Version release date (dd/mm/yyyy): 26/05/2022

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UMO.Tools.SpriteSheetCreator
{
	public class SpriteSheetCreator : EditorWindow
	{
		#region Window Variables

		static SpriteSheetCreator _window;

		Rect _leftPanel;
		Rect _rightPanel;
		Rect _bottomPanel;
		Rect _horizontalSeparator;
		Rect _verticalSeparator;

		float _leftPanelWidth = 150f;
		float _rightPanelMinWidth = 300f;
		float _bottomPanelHeight = 200f;
		float _horizontalSeparatorWidth = 5f;
		float _verticalSeparatorHeight = 5f;

		Vector2 _leftPanelScroll;
		Vector2 _rightPanelScroll;

		GUIStyle _titleStyle;
		GUIStyle _horizontalSeparatorStyle;
		GUIStyle _verticalSeparatorStyle;
		GUIStyle _layerBoxStyle;

		Texture2D _layerBoxBackgroundOdd;
		Texture2D _layerBoxBackgroundEven;
		Texture2D _layerBoxBackgroundSelected;

		#endregion

		#region Sprite Sheet Creator Variables

		int _layersCount = 0;
		int _selectedLayer = 0;

		static string _outputPath = "Assets/";
		static string _outputName = "New Sprite Sheet";

		SpriteAnchor _spriteAnchor = SpriteAnchor.MiddleCenter;

		List<List<Texture2D>> _textures = new List<List<Texture2D>>();

		#endregion

		#region Window Setup

		[MenuItem("Tools/Sprite Sheet Creator")]
		static void Init()
		{
			_window = GetWindow<SpriteSheetCreator>("Sprite Sheet Creator");
			_window.minSize = new Vector2(455f, 270f);
		}

		private void OnEnable()
		{
			InitStyles();
		}

		private void InitStyles()
		{
			_horizontalSeparatorStyle = new GUIStyle("box");
			_horizontalSeparatorStyle.border.left = _horizontalSeparatorStyle.border.right = 4;
			_horizontalSeparatorStyle.margin.left = _horizontalSeparatorStyle.margin.right = 1;
			_horizontalSeparatorStyle.padding.left = _horizontalSeparatorStyle.padding.right = 1;

			_verticalSeparatorStyle = new GUIStyle("box");
			_verticalSeparatorStyle.border.top = _verticalSeparatorStyle.border.bottom = 4;
			_verticalSeparatorStyle.margin.top = _verticalSeparatorStyle.margin.bottom = 1;
			_verticalSeparatorStyle.padding.top = _verticalSeparatorStyle.padding.bottom = 1;

			_layerBoxStyle = new GUIStyle();
			_layerBoxStyle.normal.textColor = new Color(.7f, .7f, .7f);

			_layerBoxBackgroundOdd = EditorGUIUtility.Load("builtin skins/darkskin/images/cn entrybackodd.png") as Texture2D;
			_layerBoxBackgroundEven = EditorGUIUtility.Load("builtin skins/darkskin/images/cnentrybackeven.png") as Texture2D;
			_layerBoxBackgroundSelected = EditorGUIUtility.Load("builtin skins/darkskin/images/menuitemhover.png") as Texture2D;
		}

		#endregion

		#region GUI

		private void OnGUI()
		{
			SetGUIStyles();

			DrawLeftPanel();
			DrawHorizontalSeparator();
			DrawRightPanel();
			DrawVerticalSeparator();
			DrawBottomPanel();

			if (GUI.changed) Repaint();
		}

		void SetGUIStyles()
		{
			if (_titleStyle == null)
			{
				_titleStyle = new GUIStyle("BoldLabel");
			}
		}

		void DrawLeftPanel()
		{
			_leftPanel = new Rect(2f, 0f, _leftPanelWidth, position.height - _bottomPanelHeight - _verticalSeparatorHeight);

			GUILayout.BeginArea(_leftPanel);
			_leftPanelScroll = GUILayout.BeginScrollView(_leftPanelScroll);
			GUILayout.BeginVertical();

			GUILayout.Label("Layers", _titleStyle);

			HorizontalLine();

			for (int i = 0; i < _layersCount; i++)
			{
				if (_selectedLayer == i)
				{
					DrawLayerBox($" Layer {i + 1}\n {_textures[i].Count} sprites", false, true);
					continue;
				}

				if (i % 2 == 0)
				{
					if (DrawLayerBox($" Layer {i + 1}\n {_textures[i].Count} sprites", false, false))
					{
						_selectedLayer = i;
					}
				}
				else
				{
					if (DrawLayerBox($" Layer {i + 1}\n {_textures[i].Count} sprites", true, false))
					{
						_selectedLayer = i;
					}
				}
			}

			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		void DrawRightPanel()
		{
			_rightPanel = new Rect(_leftPanelWidth + _horizontalSeparatorWidth, 0f, position.width - _leftPanel.width - _horizontalSeparatorWidth, position.height);

			GUILayout.BeginArea(_rightPanel);
			_rightPanelScroll = GUILayout.BeginScrollView(_rightPanelScroll);
			GUILayout.Label("Rows", _titleStyle);

			HorizontalLine();

			if (_textures.Count == 0)
			{
				GUILayout.EndScrollView();
				GUILayout.EndArea();
				return;
			}

			for (int i = 0; i < _textures[_selectedLayer].Count; i++)
			{
				DrawTextureBox(i);

				GUILayout.BeginHorizontal();
				GUILayout.Box(_textures[_selectedLayer][i], GUILayout.Width(40f), GUILayout.Height(40f));
				GUILayout.BeginVertical();

				if (GUILayout.Button("ADD ABOVE"))
				{
					_textures[_selectedLayer].Insert(i, null);
				}

				if (GUILayout.Button("REMOVE"))
				{
					_textures[_selectedLayer].RemoveAt(i);
				}

				GUILayout.EndVertical();
				GUILayout.BeginVertical();

				if (GUILayout.Button("UP"))
				{
					if (i != 0)
					{
						SwapTextures(i, i - 1);
					}
				}

				if (GUILayout.Button("DOWN"))
				{
					if (i != _textures[_selectedLayer].Count - 1)
					{
						SwapTextures(i, i + 1);
					}
				}

				GUILayout.EndVertical();
				GUILayout.EndHorizontal();

				Space();
				HorizontalLine();
				Space();
			}

			DrawDropArea();

			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		void DrawBottomPanel()
		{
			_bottomPanel = new Rect(2f, position.height - _bottomPanelHeight, _leftPanelWidth, _bottomPanelHeight);

			GUILayout.BeginArea(_bottomPanel);

			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();

			if (GUILayout.Button(new GUIContent("    +    ", "Add new layer at the end of the list.")))
			{
				_layersCount++;
				_textures.Add(new List<Texture2D>());
				_selectedLayer = _layersCount - 1;
			}

			if (GUILayout.Button(new GUIContent("    UP    ", "Move selected layer up.")))
			{
				if (_selectedLayer > 0)
				{
					SwapLayers(_selectedLayer, _selectedLayer - 1);
					_selectedLayer--;
				}
			}

			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();

			if (GUILayout.Button(new GUIContent("    -    ", "Remove selected layer.")))
			{
				if (_layersCount > 0)
				{
					_textures.RemoveAt(_selectedLayer);
					_layersCount--;
					if (_selectedLayer != 0)
					{
						_selectedLayer--;
					}
				}
			}

			if (GUILayout.Button(new GUIContent("DOWN", "Move selected layer down.")))
			{
				if (_selectedLayer < _layersCount - 1)
				{
					SwapLayers(_selectedLayer, _selectedLayer + 1);
					_selectedLayer++;
				}
			}

			GUILayout.EndHorizontal();

			HorizontalLine();

			GUILayout.Label("Sprite anchor point");
			_spriteAnchor = (SpriteAnchor)EditorGUILayout.EnumPopup(_spriteAnchor);

			GUILayout.Label("Export path");
			_outputPath = EditorGUILayout.TextField(_outputPath);

			GUILayout.Label("Sprite sheet name");
			_outputName = EditorGUILayout.TextField(_outputName);

			Space();

			if (GUILayout.Button("Create Sprite Sheet"))
			{
				CreateSpriteSheet();
			}

			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		void DrawHorizontalSeparator()
		{
			_horizontalSeparator = new Rect(_leftPanelWidth, 0f, _horizontalSeparatorWidth * 2f, position.height);

			if (_horizontalSeparator.x < _leftPanelWidth)
			{
				_horizontalSeparator.x = _leftPanelWidth;
			}
			else if (_horizontalSeparator.x > position.width - _rightPanelMinWidth)
			{
				_horizontalSeparator.x = position.width - _rightPanelMinWidth;
			}

			GUILayout.BeginArea(new Rect(_horizontalSeparator.position + (Vector2.right * _horizontalSeparatorWidth), new Vector2(2f, position.height)), _horizontalSeparatorStyle);
			GUILayout.EndArea();
		}

		void DrawVerticalSeparator()
		{
			_verticalSeparator = new Rect(2f, position.height - _bottomPanelHeight - _verticalSeparatorHeight * 2f, _leftPanelWidth, _verticalSeparatorHeight * 2f);

			if (_verticalSeparator.y > position.height - _bottomPanelHeight)
			{
				_verticalSeparator.y = position.height - _bottomPanelHeight;
			}

			GUILayout.BeginArea(new Rect(_verticalSeparator.position + (Vector2.up * _verticalSeparatorHeight), new Vector2(_leftPanelWidth, 2f)), _verticalSeparatorStyle);
			GUILayout.EndArea();
		}

		#endregion

		#region Layers

		bool DrawLayerBox(string content, bool isOdd, bool isSelected)
		{
			if (isSelected)
			{
				_layerBoxStyle.normal.background = _layerBoxBackgroundSelected;
			}
			else
			{
				if (isOdd)
				{
					_layerBoxStyle.normal.background = _layerBoxBackgroundOdd;
				}
				else
				{
					_layerBoxStyle.normal.background = _layerBoxBackgroundEven;
				}
			}

			return GUILayout.Button(content, _layerBoxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(32f));
		}

		void SwapLayers(int indexA, int indexB)
		{
			(_textures[indexB], _textures[indexA]) = (_textures[indexA], _textures[indexB]);
		}

		#endregion

		#region Textures List

		void DrawTextureBox(int textureIndex)
		{
			_textures[_selectedLayer][textureIndex] = EditorGUILayout.ObjectField(_textures[_selectedLayer][textureIndex], typeof(Texture2D), false) as Texture2D;
		}

		void DrawDropArea()
		{
			Event e = Event.current;

			Rect dropArea = GUILayoutUtility.GetRect(100f, 100f);

			GUI.Box(dropArea, "Drag and drop sprites");

			switch (e.type)
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:

					if (!dropArea.Contains(e.mousePosition))
					{
						break;
					}

					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

					if (e.type == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();

						foreach (Texture2D texture in DragAndDrop.objectReferences)
						{
							_textures[_selectedLayer].Add(texture);
						}
					}

					Event.current.Use();
					break;
			}
		}

		void SwapTextures(int indexA, int indexB)
		{
			(_textures[_selectedLayer][indexB], _textures[_selectedLayer][indexA]) = (_textures[_selectedLayer][indexA], _textures[_selectedLayer][indexB]);
		}

		#endregion

		#region Output

		void CreateSpriteSheet()
		{
			if (_textures == null || _textures.Count == 0)
			{
				return;
			}

			//Check export path

			string outputDir;

			if (_outputPath.EndsWith("/"))
			{
				outputDir = _outputPath.Remove(_outputPath.Length - 1);
			}
			else
			{
				outputDir = _outputPath;
				_outputPath += "/";
			}

			if (!AssetDatabase.IsValidFolder(outputDir))
			{
				if (EditorUtility.DisplayDialog("Invalid folder", $"\"{outputDir}\" is not a valid folder.", "OK"))
				{

				}
				return;
			}

			string outputFullPath = $"{_outputPath}{_outputName}.png";

			if (File.Exists(outputFullPath))
			{
				if (!EditorUtility.DisplayDialog("Override asset", $"\"{outputFullPath}\" already exists. Would you like to override it?", "Yes", "No"))
				{
					return;
				}
			}

			//Reset layers selection to avoid potential IndexOutOfBoundsException when removing empty layers.

			_selectedLayer = 0;

			//Remove empty sprites.
			//Define columns width and rows height
			//Check if at least one sprite has a different size.
			//Check if all textures are set as "readable".

			int rowHeight = _textures[0][0].height;
			int columnWidth = _textures[0][0].width;

			bool hasMultipleSizes = false;

			for (int i = _textures.Count - 1; i >= 0; i--)
			{
				if (_textures[i] == null || _textures[i].Count == 0)
				{
					_textures.RemoveAt(i);
					_layersCount--;
					continue;
				}

				for (int j = _textures[i].Count - 1; j >= 0; j--)
				{
					if (_textures[i][j] == null)
					{
						_textures[i].RemoveAt(j);
						continue;
					}

					if(!_textures[i][j].isReadable)
					{
						if (EditorUtility.DisplayDialog("Unreadable textures", "Some of the textures are not readable.\nMake sure to enable \"Read/Write\" in your textures import settings.", "OK"))
						{

						}
						return;
					}

					if (_textures[i][j].height != rowHeight || _textures[i][j].width != columnWidth)
					{
						hasMultipleSizes = true;
					}

					if (_textures[i][j].height > rowHeight)
					{
						rowHeight = _textures[i][j].height;
					}

					if (_textures[i][j].width > columnWidth)
					{
						columnWidth = _textures[i][j].width;
					}
				}
			}

			if (columnWidth == 0 || rowHeight == 0)
			{
				return;
			}

			//Define texture width

			int totalWidth = 0;
			int totalWidthDelta;

			for (int i = 0; i < _textures.Count; i++)
			{
				totalWidthDelta = columnWidth * _textures[i].Count;

				if (totalWidthDelta > totalWidth)
				{
					totalWidth = totalWidthDelta;
				}
			}

			//Define texture height

			int totalHeight = _textures.Count * rowHeight;

			//Create and fill output texture with transparent pixels

			TextureFormat _outputTextureFormat;

#if UNITY_2020_2_OR_NEWER
			_outputTextureFormat = TextureFormat.RGBA64;
#else
			_outputTextureFormat = TextureFormat.RGBA32;
#endif

			Texture2D outputTexture = new Texture2D(totalWidth, totalHeight, _outputTextureFormat, false);

			if(hasMultipleSizes)
			{
				for (int i = 0; i < outputTexture.width; i++)
				{
					for (int j = 0; j < outputTexture.height; j++)
					{
						outputTexture.SetPixel(i, j, Color.clear);
					}
				}
			}

			//Fill output texture with sprites

			int xOffset = 0;
			int yOffset = 0;

			for (int i = 0; i < _textures.Count; i++)
			{
				for (int j = 0; j < _textures[i].Count; j++)
				{
					for (int x = 0; x < _textures[i][j].width; x++)
					{
						for (int y = 0; y < _textures[i][j].height; y++)
						{
							switch (_spriteAnchor)
							{
								case SpriteAnchor.TopLeft:
									outputTexture.SetPixel(x + xOffset, totalHeight - y - yOffset - 1, _textures[i][j].GetPixel(x, _textures[i][j].height - y));
									break;
								case SpriteAnchor.TopCenter:
									outputTexture.SetPixel(x + xOffset + Mathf.RoundToInt(columnWidth * .5f - _textures[i][j].width * .5f), totalHeight - y - yOffset - 1, _textures[i][j].GetPixel(x, _textures[i][j].height - y));
									break;
								case SpriteAnchor.TopRight:
									outputTexture.SetPixel(x + xOffset + Mathf.RoundToInt(columnWidth - _textures[i][j].width), totalHeight - y - yOffset - 1, _textures[i][j].GetPixel(x, _textures[i][j].height - y));
									break;
								case SpriteAnchor.MiddleLeft:
									outputTexture.SetPixel(x + xOffset, totalHeight - y - yOffset - 1 - Mathf.RoundToInt(rowHeight * .5f - _textures[i][j].height * .5f), _textures[i][j].GetPixel(x, _textures[i][j].height - y));
									break;
								case SpriteAnchor.MiddleCenter:
									outputTexture.SetPixel(x + xOffset + Mathf.RoundToInt(columnWidth * .5f - _textures[i][j].width * .5f), totalHeight - y - yOffset - 1 - Mathf.RoundToInt(rowHeight * .5f - _textures[i][j].height * .5f), _textures[i][j].GetPixel(x, _textures[i][j].height - y));
									break;
								case SpriteAnchor.MiddleRight:
									outputTexture.SetPixel(x + xOffset + Mathf.RoundToInt(columnWidth - _textures[i][j].width), totalHeight - y - yOffset - 1 - Mathf.RoundToInt(rowHeight * .5f - _textures[i][j].height * .5f), _textures[i][j].GetPixel(x, _textures[i][j].height - y));
									break;
								case SpriteAnchor.BottomLeft:
									outputTexture.SetPixel(x + xOffset, totalHeight - y - yOffset - 1 - rowHeight + _textures[i][j].height, _textures[i][j].GetPixel(x, _textures[i][j].height - y));
									break;
								case SpriteAnchor.BottomCenter:
									outputTexture.SetPixel(x + xOffset + Mathf.RoundToInt(columnWidth * .5f - _textures[i][j].width * .5f), totalHeight - y - yOffset - 1 - rowHeight + _textures[i][j].height, _textures[i][j].GetPixel(x, _textures[i][j].height - y));
									break;
								case SpriteAnchor.BottomRight:
									outputTexture.SetPixel(x + xOffset + Mathf.RoundToInt(columnWidth - _textures[i][j].width), totalHeight - y - yOffset - 1 - rowHeight + _textures[i][j].height, _textures[i][j].GetPixel(x, _textures[i][j].height - y));
									break;
							}
						}
					}
					xOffset += columnWidth;
				}
				xOffset = 0;
				yOffset += rowHeight;
			}

			//Export texture

			byte[] bytes = outputTexture.EncodeToPNG();
			File.WriteAllBytes(outputFullPath, bytes);
			AssetDatabase.Refresh();
		}

		#endregion

		#region Enums

		enum SpriteAnchor
		{
			TopLeft,
			TopCenter,
			TopRight,
			MiddleLeft,
			MiddleCenter,
			MiddleRight,
			BottomLeft,
			BottomCenter,
			BottomRight
		}

		#endregion

		#region GUI Utility

		void HorizontalLine()
		{
			GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
		}

		void Space()
		{
			GUILayout.Space(10f);
		}

		#endregion
	}
}