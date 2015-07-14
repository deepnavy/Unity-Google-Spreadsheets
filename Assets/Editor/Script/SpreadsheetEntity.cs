using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Google.GData.Client;
using Google.GData.Spreadsheets;

[InitializeOnLoad]
public class SpreadsheetEntity : MonoBehaviour 
{
	// enter Client ID and Client Secret values
	const string _ClientId = "";
	const string _ClientSecret = "";
	// enter Access Code after getting it from auth url
	const string _AccessCode = "";
	// enter Auth 2.0 Refresh Token and AccessToken after succesfully authorizing with Access Code
	const string _RefreshToken = "";
	const string _AccessToken = "";

	const string _SpreadsheetName = "";


	static SpreadsheetsService service;

	
	public static GOAuth2RequestFactory RefreshAuthenticate() 
	{
		OAuth2Parameters parameters = new OAuth2Parameters()
		{
			RefreshToken = _RefreshToken,
			AccessToken = _AccessToken,
			ClientId = _ClientId,
			ClientSecret = _ClientSecret,
			Scope = "https://www.googleapis.com/auth/drive https://spreadsheets.google.com/feeds",
			AccessType = "offline",
			TokenType = "refresh"
		};
		string authUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);
		return new GOAuth2RequestFactory("spreadsheet", "MySpreadsheetIntegration-v1", parameters);
	}

	static void Auth()
	{
		GOAuth2RequestFactory requestFactory = RefreshAuthenticate();
		
		service = new SpreadsheetsService("MySpreadsheetIntegration-v1");  
		service.RequestFactory = requestFactory;
	}
	

	// Use this for initialization
	static SpreadsheetEntity(){
		if (_RefreshToken == "" && _AccessToken == "")
		{
			Init();
			return;
		}
		
		Auth();
		
		Google.GData.Spreadsheets.SpreadsheetQuery query = new Google.GData.Spreadsheets.SpreadsheetQuery();
		
		// Make a request to the API and get all spreadsheets.
		SpreadsheetFeed feed = service.Query(query);
		
		if (feed.Entries.Count == 0)
		{
			Debug.Log("There are no spreadsheets in your docs.");
			return;
		}
		
		AccessSpreadsheet(feed);
	}

	// access spreadsheet data
	static void AccessSpreadsheet(SpreadsheetFeed feed)
	{

		string name = _SpreadsheetName;
		SpreadsheetEntry spreadsheet = null;

		foreach (AtomEntry sf in feed.Entries)
		{
			if (sf.Title.Text == name)
			{
				spreadsheet = (SpreadsheetEntry)sf;
			}
		}

		if (spreadsheet == null)
		{
			Debug.Log("There is no such spreadsheet with such title in your docs.");
			return;
		}

		
		// Get the first worksheet of the first spreadsheet.
		WorksheetFeed wsFeed = spreadsheet.Worksheets;
		WorksheetEntry worksheet = (WorksheetEntry)wsFeed.Entries[0];
		
		// Define the URL to request the list feed of the worksheet.
		AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);
		
		// Fetch the list feed of the worksheet.
		ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
		ListFeed listFeed = service.Query(listQuery);


		foreach (ListEntry row in listFeed.Entries)
		{
			//access spreadsheet data here
		}


	}
	
	static void Init()
	{
		
		////////////////////////////////////////////////////////////////////////////
		// STEP 1: Configure how to perform OAuth 2.0
		////////////////////////////////////////////////////////////////////////////

		if (_ClientId == "" && _ClientSecret == "")
		{
			Debug.Log("Please paste Client ID and Client Secret");
			return;
		}

		string CLIENT_ID = _ClientId;

		string CLIENT_SECRET = _ClientSecret;

		string SCOPE = "https://www.googleapis.com/auth/drive https://spreadsheets.google.com/feeds https://docs.google.com/feeds";

		string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";
		
		string TOKEN_TYPE = "refresh";
		
		////////////////////////////////////////////////////////////////////////////
		// STEP 2: Set up the OAuth 2.0 object
		////////////////////////////////////////////////////////////////////////////
		
		// OAuth2Parameters holds all the parameters related to OAuth 2.0.
		OAuth2Parameters parameters = new OAuth2Parameters();

		parameters.ClientId = CLIENT_ID;

		parameters.ClientSecret = CLIENT_SECRET;

		parameters.RedirectUri = REDIRECT_URI;
		
		////////////////////////////////////////////////////////////////////////////
		// STEP 3: Get the Authorization URL
		////////////////////////////////////////////////////////////////////////////

		parameters.Scope = SCOPE;
		
		parameters.AccessType = "offline"; // IMPORTANT and was missing in the original
		
		parameters.TokenType = TOKEN_TYPE; // IMPORTANT and was missing in the original

		// Authorization url.

		string authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);
		Debug.Log(authorizationUrl);
		Debug.Log("Please visit the URL above to authorize your OAuth "
		          + "request token.  Once that is complete, type in your access code to "
		          + "continue...");

		parameters.AccessCode = _AccessCode;

		if (parameters.AccessCode == "")
		{
			Application.OpenURL(authorizationUrl);
			return;
		}
		////////////////////////////////////////////////////////////////////////////
		// STEP 4: Get the Access Token
		////////////////////////////////////////////////////////////////////////////

		OAuthUtil.GetAccessToken(parameters);
		string accessToken = parameters.AccessToken;
		string refreshToken = parameters.RefreshToken;
		Debug.Log("OAuth Access Token: " + accessToken + "\n");
		Debug.Log("OAuth Refresh Token: " + refreshToken + "\n");
	
	}
	
}

