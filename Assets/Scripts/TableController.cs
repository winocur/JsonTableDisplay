using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

// -- By Manuel Winocur -- //
public class TableController : MonoBehaviour {

	private const string FILE_NAME = "JsonChallenge.json";
	private string filePath;

	// keys for json parsing
	private const string TABLE_TITLE   = "Title";
	private const string TABLE_HEADERS = "ColumnHeaders";
	private const string TABLE_DATA    = "Data";
	
	public TableView tableView;

	private FileSystemWatcher watcher;

	private bool triggerReload;

	// Use this for initialization
	void Start () {

		filePath = Application.streamingAssetsPath + "/" + FILE_NAME;

		LoadAndDisplayJsonTable();

		// register callback to filesystem to update the table view
		// when the file is modified
		watcher = new FileSystemWatcher();
		watcher.Path = Application.streamingAssetsPath;
		watcher.Changed += OnJsonFileChanged;
		watcher.EnableRaisingEvents = true;
	}

	//	Event called from FileSystemWatcher when the json table
	// file has changed to do an automated reload.
	private void OnJsonFileChanged (object sender, FileSystemEventArgs e) {
		
		// We need to do this so synchronize the event with the
		// Unity main thread so we can operate on the UI
		lock(this) {	
			triggerReload = true;
		}
	}

	void Update () {

		// reload synchronization with FileSystemWatcher
		if(triggerReload) {
			lock(this) {
				triggerReload = false;
				LoadAndDisplayJsonTable();
			}
		}
	}
	
	private void LoadAndDisplayJsonTable ()
	{
		try {
			string fileContent = File.ReadAllText(filePath);

			TableModel tableModel = new TableModel();
			JObject tableJson = JObject.Parse(fileContent);

			tableModel.Title = tableJson.Value<string>(TABLE_TITLE);

			JArray tableHeaders = tableJson.Value<JArray>(TABLE_HEADERS);
			int columnCount = tableHeaders.Count;

			tableModel.ColumnHeaders = new string [columnCount];
			for (int i = 0; i < columnCount; i++) {
				tableModel.ColumnHeaders[i] = tableHeaders.GetItem(i).Value<string>();
			}

			JArray dataElements = tableJson.Value<JArray>(TABLE_DATA);
			tableModel.Data = new TableDataEntry [dataElements.Count];
			for (int i = 0; i < dataElements.Count; i++) {
				List<string> propertyValues = new List<string>();
				JObject dataElement = (JObject)dataElements.GetItem(i);
				foreach (var property in dataElement.Properties()) {
					propertyValues.Add(property.Value.ToString());
				}

				if(propertyValues.Count != columnCount) {
					tableView.ShowError(string.Format("Data element in row {0} doesn't match the header count: {1}", i, columnCount));
					return;
				}

				tableModel.Data[i].Values = propertyValues.ToArray();
			}

			tableView.ShowTable(tableModel);

		} catch (Exception ex) {
			tableView.ShowError(ex.Message);
		}

	}
}
