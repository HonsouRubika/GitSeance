/// Unity Modules - Field Tracker
/// Created by: Nicolas Capelier
/// Contact: capelier.nicolas@gmail.com
/// Version: 0.2.0
/// Version release date (dd/mm/yyyy): 10/07/2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UMO.Tools.FieldTracker
{
	public class FieldTrackerEditorWindow : EditorWindow
	{
		#region Window Variables

		Rect _leftPanel;
		Rect _rightPanel;
		Rect _bottomPanel;
		Rect _horizontalSeparator;
		Rect _verticalSeparator;

		float _leftPanelWidth = 150f;
		float _rightPanelMinWidth = 300f;
		float _bottomPanelHeight = 80f;
		float _horizontalSeparatorWidth = 5f;
		float _verticalSeparatorHeight = 5f;

		Vector2 _leftPanelScroll;
		Vector2 _rightPanelScroll;

		GUIStyle _titleStyle;
		GUIStyle _componentStyle;
		GUIStyle _horizontalSeparatorStyle;
		GUIStyle _verticalSeparatorStyle;
		GUIStyle _trackedGameObjectBoxStyle;
		GUIStyle _trackedFieldBoxStyle;

		Texture2D _trackedGameObjectBoxBackgroundOdd;
		Texture2D _trackedGameObjectBoxBackgroundEven;
		Texture2D _trackedGameObjectBoxBackgroundSelected;
		Texture2D _trackedFieldBoxBackgroundOdd;
		Texture2D _trackedFieldBoxBackgroundEven;

		#endregion

		#region Field Tracker Variables

		List<TrackedGameObject> _trackedGameObjects = new List<TrackedGameObject>();

		const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		static bool _showFieldType = true;
		static bool _enableLiveFieldUpdate = false;

		int _trackedGameObjectsCount = 0;
		int _selectedTrackedGameObject = 0;

		#endregion

		#region Window Setup

		[MenuItem("Tools/Field Tracker")]
		static void Init()
		{
			FieldTrackerEditorWindow window = GetWindow<FieldTrackerEditorWindow>("Field Tracker");
			window.minSize = new Vector2(455f, 270f);
		}

		private void OnEnable()
		{
			RefreshTrackedMembers();
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

			_trackedGameObjectBoxStyle = new GUIStyle();
			_trackedGameObjectBoxStyle.normal.textColor = new Color(.7f, .7f, .7f);

			_trackedGameObjectBoxBackgroundOdd = EditorGUIUtility.Load("builtin skins/darkskin/images/cn entrybackodd.png") as Texture2D;
			_trackedGameObjectBoxBackgroundEven = EditorGUIUtility.Load("builtin skins/darkskin/images/cnentrybackeven.png") as Texture2D;
			_trackedGameObjectBoxBackgroundSelected = EditorGUIUtility.Load("builtin skins/darkskin/images/menuitemhover.png") as Texture2D;

			_trackedFieldBoxStyle = new GUIStyle() { alignment = TextAnchor.MiddleLeft };
			_trackedFieldBoxStyle.normal.textColor = Color.white;

			_trackedFieldBoxBackgroundOdd = EditorGUIUtility.Load("builtin skins/darkskin/images/cn entrybackodd.png") as Texture2D;
			_trackedFieldBoxBackgroundEven = EditorGUIUtility.Load("builtin skins/darkskin/images/cnentrybackeven.png") as Texture2D;
		}

		#endregion

		#region GUI

		private void Update()
		{
			if (_enableLiveFieldUpdate)
				Repaint();
		}

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

		private void SetGUIStyles()
		{
			if (_titleStyle == null)
			{
				_titleStyle = new GUIStyle("BoldLabel");
			}

			if (_componentStyle == null)
			{
				_componentStyle = new GUIStyle("BoldLabel") { alignment = TextAnchor.MiddleCenter };
			}
		}

		void DrawLeftPanel()
		{
			_leftPanel = new Rect(2f, 0f, _leftPanelWidth, position.height - _bottomPanelHeight - _verticalSeparatorHeight);

			GUILayout.BeginArea(_leftPanel);
			_leftPanelScroll = GUILayout.BeginScrollView(_leftPanelScroll);
			GUILayout.BeginVertical();

			GUILayout.Label("Tracked Game Objects", _titleStyle);

			HorizontalLine();

			for (int i = 0; i < _trackedGameObjectsCount; i++)
			{
				if (_selectedTrackedGameObject == i)
				{
					DrawTrackedGameObjectBox($" {_trackedGameObjects[i].gameObject.name}\n {_trackedGameObjects[i].components.Length} components", false, true);
					continue;
				}

				if (i % 2 == 0)
				{
					if (DrawTrackedGameObjectBox($" {_trackedGameObjects[i].gameObject.name}\n {_trackedGameObjects[i].components.Length} components", false, false))
					{
						_selectedTrackedGameObject = i;
					}
				}
				else
				{
					if (DrawTrackedGameObjectBox($" {_trackedGameObjects[i].gameObject.name}\n {_trackedGameObjects[i].components.Length} components", true, false))
					{
						_selectedTrackedGameObject = i;
					}
				}
			}

			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		void DrawRightPanel()
		{
			_rightPanel = new Rect(_leftPanelWidth + _horizontalSeparatorWidth * 2f, 0f, position.width - _leftPanel.width - _horizontalSeparatorWidth, position.height);

			GUILayout.BeginArea(_rightPanel);
			_rightPanelScroll = GUILayout.BeginScrollView(_rightPanelScroll);

			GUILayout.Label("Tracked Fields", _titleStyle);

			HorizontalLine();

			if (_trackedGameObjectsCount == 0)
			{
				GUILayout.EndScrollView();
				GUILayout.EndArea();
				return;
			}

			foreach (TrackedComponent component in _trackedGameObjects[_selectedTrackedGameObject].components)
			{
				EditorGUILayout.LabelField(component.component.GetType().ToString(), _componentStyle);

				Space();

				for (int i = 0; i < component.fields.Length; i++)
				{
					if (i % 2 == 0)
					{
						if (string.IsNullOrEmpty(component.fields[i].displayName))
						{
							DrawTrackedField(component.fields[i].field.Name, component.fields[i].field.GetValue(component.component).ToString(), component.fields[i].field.FieldType.ToString(), false);
						}
						else
						{
							DrawTrackedField(component.fields[i].displayName, component.fields[i].field.GetValue(component.component).ToString(), component.fields[i].field.FieldType.ToString(), false);
						}
					}
					else
					{
						if (string.IsNullOrEmpty(component.fields[i].displayName))
						{
							DrawTrackedField(component.fields[i].field.Name, component.fields[i].field.GetValue(component.component).ToString(), component.fields[i].field.FieldType.ToString(), true);
						}
						else
						{
							DrawTrackedField(component.fields[i].displayName, component.fields[i].field.GetValue(component.component).ToString(), component.fields[i].field.FieldType.ToString(), true);
						}
					}
				}

				HorizontalLine();
			}

			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		void DrawBottomPanel()
		{
			_bottomPanel = new Rect(2f, position.height - _bottomPanelHeight, _leftPanelWidth, _bottomPanelHeight);

			GUILayout.BeginArea(_bottomPanel);

			GUILayout.BeginVertical();

			_showFieldType = GUILayout.Toggle(_showFieldType, "Show Field Type");

			_enableLiveFieldUpdate = GUILayout.Toggle(_enableLiveFieldUpdate, "Live Field Update");

			if (GUILayout.Button("Ping Selected"))
			{
				if (_trackedGameObjectsCount > 0)
					Ping(_trackedGameObjects[_selectedTrackedGameObject].gameObject);
			}

			if (GUILayout.Button("Refresh Tracked Fields"))
			{
				RefreshTrackedMembers();
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

		#region Tracked GameObjects

		bool DrawTrackedGameObjectBox(string content, bool isOdd, bool isSelected)
		{
			if (isSelected)
			{
				_trackedGameObjectBoxStyle.normal.background = _trackedGameObjectBoxBackgroundSelected;
			}
			else
			{
				if (isOdd)
				{
					_trackedGameObjectBoxStyle.normal.background = _trackedGameObjectBoxBackgroundOdd;
				}
				else
				{
					_trackedGameObjectBoxStyle.normal.background = _trackedGameObjectBoxBackgroundEven;
				}
			}

			return GUILayout.Button(content, _trackedGameObjectBoxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(32f));
		}

		#endregion

		#region Tracked Fields

		void DrawTrackedField(string fieldName, string fieldValue, string fieldType, bool isOdd)
		{
			string content;

			if (_showFieldType)
			{
				content = $" {fieldName} = {fieldValue}\n <color=grey>({fieldType})</color>";
			}
			else
			{
				content = $" {fieldName} = {fieldValue}";
			}

			if (isOdd)
			{
				_trackedFieldBoxStyle.normal.background = _trackedFieldBoxBackgroundOdd;
			}
			else
			{
				_trackedFieldBoxStyle.normal.background = _trackedFieldBoxBackgroundEven;
			}

			GUILayout.Button(content, _trackedFieldBoxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(32f));
		}

		#endregion

		#region Input

		private void RefreshTrackedMembers()
		{
			_trackedGameObjects = new List<TrackedGameObject>();

			List<TrackedComponent> trackedComponents = new List<TrackedComponent>();
			List<TrackedField> trackedFields = new List<TrackedField>();

			GameObject[] gameObjectsInScene = FindObjectsOfType<GameObject>();

			foreach (GameObject gameObject in gameObjectsInScene)
			{
				trackedComponents.Clear();

				Component[] components = gameObject.GetComponents<Component>();
				foreach (Component component in components)
				{
					Type type = component.GetType();

					if (!type.GetCustomAttributes<TrackedComponentAttribute>(true).Any())
						continue;

					trackedFields.Clear();

					FieldInfo[] fields = type.GetFields(FLAGS);

					if (type.BaseType != typeof(Component))
					{
						Type currentType = type;
						FieldInfoComparer fieldComparer = new FieldInfoComparer();
						HashSet<FieldInfo> fieldInfoList = new HashSet<FieldInfo>(fields, fieldComparer);

						while (currentType != typeof(Component))
						{
							fields = currentType.GetFields(FLAGS);
							fieldInfoList.UnionWith(fields);
							currentType = currentType.BaseType;
						}

						fields = fieldInfoList.ToArray();
					}

					foreach (FieldInfo field in fields)
					{
						if (field.CustomAttributes.ToArray().Length > 0)
						{
							TrackedFieldAttribute attribute = field.GetCustomAttribute<TrackedFieldAttribute>(true);

							if (attribute != null)
							{
								trackedFields.Add(new TrackedField(field, attribute._displayName));
							}
						}
					}

					if (trackedFields.Count > 0)
					{
						trackedComponents.Add(new TrackedComponent(component, trackedFields.ToArray()));
					}
				}

				if (trackedComponents.Count > 0)
				{
					_trackedGameObjects.Add(new TrackedGameObject(gameObject, trackedComponents.ToArray()));
				}
			}

			_trackedGameObjectsCount = _trackedGameObjects.Count;
		}

		class FieldInfoComparer : IEqualityComparer<FieldInfo>
		{
			public bool Equals(FieldInfo a, FieldInfo b)
			{
				return a.DeclaringType == b.DeclaringType && a.Name == b.Name;
			}

			public int GetHashCode(FieldInfo obj)
			{
				return obj.Name.GetHashCode() ^ obj.DeclaringType.GetHashCode();
			}
		}

		#endregion

		#region Highlight

		void Ping(GameObject go)
		{
			EditorGUIUtility.PingObject(go);
		}

		#endregion

		#region Structs

		struct TrackedGameObject
		{
			public GameObject gameObject;
			public TrackedComponent[] components;

			public TrackedGameObject(GameObject gameObject, TrackedComponent[] components)
			{
				this.gameObject = gameObject;
				this.components = components;
			}
		}

		struct TrackedComponent
		{
			public Component component;
			public TrackedField[] fields;

			public TrackedComponent(Component component, TrackedField[] fields)
			{
				this.component = component;
				this.fields = fields;
			}
		}

		struct TrackedField
		{
			public FieldInfo field;
			public string displayName;

			public TrackedField(FieldInfo field, string displayName)
			{
				this.field = field;
				this.displayName = displayName;
			}
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