using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// -- By Manuel Winocur -- //
public class TableView : MonoBehaviour {

	public Color textColor;
	public Font textFont;
	public int textSize;
	public int headerTextSize;
	public int rowHeight;

	private HorizontalLayoutGroup tableContainer;
	private Text errorText;

	//NOTE: This could have been made with prefabs but the challenge explicity says "instantiate UI components"
	public void ShowTable (TableModel model) {
		
		ClearView();

		// set up table container
		{
			var tableContainerTransform = new GameObject().AddComponent<RectTransform>();
			tableContainerTransform.name = "table_container";
			tableContainerTransform.SetParent(this.transform);
			tableContainerTransform.anchoredPosition = this.GetComponent<RectTransform>().anchoredPosition;
			tableContainerTransform.sizeDelta = this.GetComponent<RectTransform>().sizeDelta;
			tableContainer = tableContainerTransform.gameObject.AddComponent<HorizontalLayoutGroup>();
		}

		for (int i = 0; i < model.ColumnHeaders.Length; i++) {
			
			// -- create new column -- //
			GameObject column = new GameObject();
			column.name = "column_i";
			var columnLayout = column.AddComponent<VerticalLayoutGroup>();	
			columnLayout.transform.SetParent (tableContainer.transform);

			// -- header section -- //
			Text headerText = ConfigureText(new GameObject().AddComponent<Text>(), true);
			headerText.gameObject.name = "header_" + i;
			headerText.gameObject.transform.SetParent(columnLayout.transform);
			headerText.text = "<b>" + model.ColumnHeaders[i] + "</b>"; 

			// -- data fields -- //
			for (int j = 0; j < model.Data.Length; j++) {
				Text dataText = ConfigureText(new GameObject().AddComponent<Text>());
				dataText.text = model.Data[j].Values[i];
				dataText.transform.SetParent(columnLayout.transform);
			}
		}
	}

	public void ShowError (string errorMessage) {
		ClearView();

		errorText = ConfigureText(new GameObject().AddComponent<Text>());
		errorText.rectTransform.sizeDelta = this.GetComponent<RectTransform>().sizeDelta;
		errorText.transform.SetParent(this.transform);
		errorText.color = Color.red;
		errorText.resizeTextMaxSize = 40;
		errorText.text = "ERROR: " + errorMessage;
	}

    private void ClearView()
    {
        if(this.tableContainer != null)
           	Destroy(this.tableContainer.gameObject);
		
		if(this.errorText != null)
			Destroy(this.errorText.gameObject);
    }

    private Text ConfigureText (Text text, bool isHeader = false) {

		text.font = textFont;
		text.color = textColor;
		text.fontSize = (isHeader) ? headerTextSize : textSize;
		text.resizeTextForBestFit = true;
		text.resizeTextMaxSize = text.fontSize;
		text.alignment = TextAnchor.MiddleCenter;
		return text;
	}
}
