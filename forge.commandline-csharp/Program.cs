//
// Copyright (c) 2017 Autodesk, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// by Cyrille Fauvel
// Autodesk Forge Partner Development
// January 2017
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://www.nuget.org/packages/CommandLineParser/2.1.1-beta
// https://github.com/gsscoder/commandline/wiki/Latest-Version#verbs
using CommandLine;
using CommandLine.Text;

using Autodesk.Forge;
using Autodesk.Forge.Client;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Autodesk.Forge.Model;
using System.IO;
using Newtonsoft.Json.Linq;

namespace forge.commandline_csharp {

	#region Command Definitions
	[Verb ("2legged", HelpText = "get an application access token (2 legged)")]
	class TwoLeggedOptions {
	}

	[Verb ("3legged", HelpText = "get an user access token (3 legged)")]
	class ThreeLeggedOptions {
		//[Option('c', "code", SetName = "code", HelpText = "code returned to you after user authentication")]
		[Value(0, HelpText = "code returned to you after user authentication")]
        public string code { get; set; }
	}

	[Verb ("aboutme", HelpText = "3legged aboutme information")]
	class AboutMeOptions {
	}

	[Verb ("buckets", HelpText = "list local/server buckets")]
	class BucketsOptions {
		[Option('s', "server", SetName = "server", Default = false, HelpText = "list from server vs local")]
		public bool server { get; set; }
		[Option('a', "startAt", SetName = "startAt", HelpText = "startAt: where to start in the list [string, default: none]")]
		public string startAt { get; set; }
		[Option('l', "limit", SetName = "limit", Default = 10, HelpText = "limit: how many to return [integer, default: 10]")]
		public int limit { get; set; }
		[Option('r', "region", SetName = "region", Default = "US", HelpText = "region: US or EMEA [string, default: US]")]
		public string region { get; set; }
	}

	[Verb ("bucket", HelpText = "set or get the current bucket")]
	class BucketOptions {
		[Value(0, HelpText = "bucket name")]
        public string bucket { get; set; }
	}

	[Verb ("bucketCreate", HelpText = "create a new bucket,; default Type is Transient, values can be Transient/Temporary/Persistent")]
	class BucketCreateOptions {
		[Value(0, HelpText = "bucket name")]
        public string bucket { get; set; }
		[Option('r', "region", SetName = "region", Default = "US", HelpText = "region: US or EMEA [string, default: US]")]
		public string region { get; set; }
		[Option('t', "type", SetName = "type", Default = PostBucketsPayload.PolicyKeyEnum.Transient, HelpText = "type: Transient or Temporary or Persistent [string, default: Transient]")]
		public  PostBucketsPayload.PolicyKeyEnum type { get; set; }
	}

	[Verb ("bucketCheck", HelpText = "check bucket validity, outputs the expiration; date/time for this bucket; if no parameter use the current bucket")]
	class BucketCheckOptions {
		[Value(0, HelpText = "bucket name")]
        public string bucket { get; set; }
	}

	[Verb ("bucketItems", HelpText = "list items in a given bucket; required to be in the API white list to use this API; if no parameter use the current bucket")]
	class BucketItemsOptions {
		[Value(0, HelpText = "bucket name")]
        public string bucket { get; set; }
		[Option('a', "startAt", SetName = "startAt", HelpText = "startAt: where to start in the list [string, default: none]")]
		public string startAt { get; set; }
		[Option('l', "limit", SetName = "limit", Default = 10, HelpText = "limit: how many to return [integer, default: 10]")]
		public int limit { get; set; }
		[Option('r', "region", SetName = "region", Default = "US", HelpText = "region: US or EMEA [string, default: US]")]
		public string region { get; set; }
	}

	[Verb ("bucketCheck", HelpText = "delete a given bucket; if no parameter delete the current bucket")]
	class BucketDeleteOptions {
		[Value(0, HelpText = "bucket name")]
        public string bucket { get; set; }
	}

	[Verb ("upload", HelpText = "upload a file in the current bucket")]
	class UploadOptions {
		[Value(0, HelpText = "file name")]
        public string filename { get; set; }
	}

	[Verb ("resumable", HelpText = "upload a file in multiple pieces (i.e. resumables)")]
	class ResumableOptions {
		[Value(0, HelpText = "file name")]
        public string filename { get; set; }
		[Value(1, HelpText = "split the file in N pieces")]
		public int pieces { get; set; }
	}

	[Verb ("download", HelpText = "download the file from OSS")]
	class DownloadOptions {
		[Value(0, HelpText = "file key")]
        public string fileKey { get; set; }
		[Value(1, HelpText = "output file name")]
        public string outputFile { get; set; }
		[Option('f', "file", SetName = "file", Default = false, HelpText = "fileKey represent the final objectKey on OSS vs a local fileKey")]
		public bool file { get; set; }
	}

	[Verb ("objectDetails", HelpText = "file information")]
	class ObjectDetailsOptions {
		[Value(0, HelpText = "file key")]
        public string fileKey { get; set; }
		[Option('f', "file", SetName = "file", Default = false, HelpText = "fileKey represent the final objectKey on OSS vs a local fileKey")]
		public bool file { get; set; }
	}

	[Verb ("translate", HelpText = "list items in a given bucket; required to be in the API white list to use this API; if no parameter use the current bucket")]
	class TranslateOptions {
		[Value(0, HelpText = "file key")]
        public string fileKey { get; set; }
		[Option('f', "file", SetName = "file", Default = false, HelpText = "fileKey represent the final objectKey on OSS vs a local fileKey")]
		public bool file { get; set; }
		[Option('e', "entry", SetName = "entry", HelpText = "rootFile: which file to start from")]
		public string entry { get; set; }
	}

	[Verb ("translateProgress", HelpText = "file translation progress")]
	class TranslateProgressOptions {
		[Value(0, HelpText = "file key")]
        public string fileKey { get; set; }
		[Option('f', "file", SetName = "file", Default = false, HelpText = "fileKey represent the final objectKey on OSS vs a local fileKey")]
		public bool file { get; set; }
	}

	[Verb ("manifest", HelpText = "file manifest")]
	class ManifestOptions {
		[Value(0, HelpText = "file key")]
        public string fileKey { get; set; }
		[Option('f', "file", SetName = "file", Default = false, HelpText = "fileKey represent the final objectKey on OSS vs a local fileKey")]
		public bool file { get; set; }
	}

	[Verb ("metadata", HelpText = "file metadata")]
	class MetadataOptions {
		[Value(0, HelpText = "file key")]
        public string fileKey { get; set; }
		[Option('f', "file", SetName = "file", Default = false, HelpText = "fileKey represent the final objectKey on OSS vs a local fileKey")]
		public bool file { get; set; }
	}

	[Verb ("thumbnail", HelpText = "get thumbnail")]
	class ThumbnailOptions {
		[Value(0, HelpText = "file key")]
        public string fileKey { get; set; }
		[Value(1, HelpText = "output file name")]
        public string outputFile { get; set; }
		[Option('f', "file", SetName = "file", Default = false, HelpText = "fileKey represent the final objectKey on OSS vs a local fileKey")]
		public bool file { get; set; }
	}

	[Verb ("deleteFile", HelpText = "delete the source file from the bucket")]
	class DeleteFileOptions {
		[Value(0, HelpText = "file key")]
        public string fileKey { get; set; }
		[Option('f', "file", SetName = "file", Default = false, HelpText = "fileKey represent the final objectKey on OSS vs a local fileKey")]
		public bool file { get; set; }
	}

	[Verb ("deleteManifest", HelpText = "delete the manifest and all its translated output files (derivatives)")]
	class DeleteManifestOptions {
		[Value(0, HelpText = "file key")]
        public string fileKey { get; set; }
		[Option('f', "file", SetName = "file", Default = false, HelpText = "fileKey represent the final objectKey on OSS vs a local fileKey")]
		public bool file { get; set; }
	}

	[Verb ("html", HelpText = "generate default html page")]
	class HtmlOptions {
		[Value(0, HelpText = "file key")]
        public string fileKey { get; set; }
		[Value(1, HelpText = "output file name")]
        public string outputFile { get; set; }
		[Option('f', "file", SetName = "file", Default = false, HelpText = "fileKey represent the final objectKey on OSS vs a local fileKey")]
		public bool file { get; set; }
	}

	#endregion

	class Program {
		static void Main (string [] args) {
			// Limited to 16
			//Parser.Default.ParseArguments<
			//			TwoLeggedOptions,
			//			ThreeLeggedOptions
			//		> (args)
			//	.MapResult (
			//		(TwoLeggedOptions opts) => RunTwoLegged (opts),
			//		(ThreeLeggedOptions opts) => RunThreeLegged (opts),
			//		errs => 1
			//	) ;
			Parser.Default.ParseArguments (args, new [] {
				typeof (TwoLeggedOptions),
				typeof (ThreeLeggedOptions),
				typeof (AboutMeOptions),
				typeof (BucketsOptions),
				typeof (BucketOptions),
				typeof (BucketCreateOptions),
				typeof (BucketCheckOptions),
				typeof (BucketItemsOptions),
				typeof (BucketDeleteOptions),
				typeof (UploadOptions),
				typeof (ResumableOptions),
				typeof (DownloadOptions),
				typeof (ObjectDetailsOptions),
				typeof (TranslateOptions),
				typeof (TranslateProgressOptions),
				typeof (ManifestOptions),
				typeof (MetadataOptions),
				typeof (ThumbnailOptions),
				typeof (DeleteFileOptions),
				typeof (DeleteManifestOptions),
				typeof (HtmlOptions)
			})
				.WithParsed<TwoLeggedOptions> (opts => RunTwoLegged (opts))
				.WithParsed<ThreeLeggedOptions> (opts => RunThreeLegged (opts))
				.WithParsed<AboutMeOptions> (opts => RunAboutMe (opts))
				.WithParsed<BucketsOptions> (opts => RunBuckets (opts))
				.WithParsed<BucketOptions> (opts => RunBucket (opts))
				.WithParsed<BucketCreateOptions> (opts => RunBucketCreate (opts))
				.WithParsed<BucketCheckOptions> (opts => RunBucketCheck (opts))
				.WithParsed<BucketItemsOptions> (opts => RunBucketItems (opts))
				.WithParsed<BucketDeleteOptions> (opts => RunBucketDelete (opts))
				.WithParsed<UploadOptions> (opts => RunUpload (opts))
				.WithParsed<ResumableOptions> (opts => RunResumable (opts))
				.WithParsed<DownloadOptions> (opts => RunDownload (opts))
				.WithParsed<ObjectDetailsOptions> (opts => RunObjectDetails (opts))
				.WithParsed<TranslateOptions> (opts => RunTranslate (opts))
				.WithParsed<TranslateProgressOptions> (opts => RunTranslateProgress (opts))
				.WithParsed<ManifestOptions> (opts => RunManifest (opts))
				.WithParsed<MetadataOptions> (opts => RunMetadata (opts))
				.WithParsed<ThumbnailOptions> (opts => RunThumbnail (opts))
				.WithParsed<DeleteFileOptions> (opts => RunDeleteFile (opts))
				.WithParsed<DeleteManifestOptions> (opts => RunDeleteManifest (opts))
				.WithParsed<HtmlOptions> (opts => RunHtml (opts))
				.WithNotParsed (errs => {}) ;
		}

		#region Initialization
		private static string FORGE_CLIENT_ID ="" ; // 'your_client_id'
		private static string FORGE_CLIENT_SECRET ="" ; // 'your_client_secret'
		private static string PORT ="" ; // 3006
		private static string FORGE_CALLBACK =null ; // 'http://localhost:' + PORT + '/oauth' ;

		private static string grantType ="client_credentials" ; // {String} Must be ``client_credentials``
		private static Scope[] scope =new Scope[] { Scope.DataRead, Scope.DataWrite, Scope.DataCreate,
				Scope.DataSearch, Scope.BucketCreate, Scope.BucketRead, Scope.BucketUpdate, Scope.BucketDelete } ;
		private static Scope[] scopeViewer =new Scope[] { Scope.DataRead } ;

		internal static bool readKeys () {
			FORGE_CLIENT_ID =Environment.GetEnvironmentVariable ("FORGE_CLIENT_ID") ;
			FORGE_CLIENT_SECRET =Environment.GetEnvironmentVariable ("FORGE_CLIENT_SECRET") ;
			PORT =Environment.GetEnvironmentVariable ("PORT") ;
			if ( PORT == null )
				PORT ="3006" ;
			if ( FORGE_CALLBACK == null ) {
				FORGE_CALLBACK =Environment.GetEnvironmentVariable ("FORGE_CALLBACK") ;
				if ( FORGE_CALLBACK == null )
					FORGE_CALLBACK ="http://localhost:" + PORT + "/oauth" ;
			}

			return (true) ;
		}

		#endregion

		#region Access Token
		// One example for async implementation - do not use in a console application
		internal async static Task<ApiResponse<dynamic>> oauthExecAsync () {
			TwoLeggedApi apiInstance =new TwoLeggedApi () ;
			ApiResponse<dynamic> bearer =null ;
			try {
				readKeys () ;
				bearer =await apiInstance.AuthenticateAsyncWithHttpInfo (FORGE_CLIENT_ID, FORGE_CLIENT_SECRET, grantType, scope) ;
				if ( httpErrorHandler (bearer, "Failed to get your token", false) ) {
					System.IO.File.Delete ("data/access_token") ;
					return (null) ;
				}
				//Console.WriteLine (bearer.Data.ToString () as string) ;
				string token =bearer.Data.token_type + " " + bearer.Data.access_token ;
				Console.WriteLine ("Your new 2-legged access token is: " + token) ;
				DateTime dt =DateTime.Now ;
				dt.AddSeconds (double.Parse (bearer.Data.expires_in)) ;
				Console.WriteLine ("Expires at: " + dt.ToLocalTime ()) ;
				System.IO.File.WriteAllText ("/data/access_token", token) ;
				return (bearer) ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling TwoLeggedApi.Authenticate: " + ex.Message) ;
				return (null) ;
			}
		}

		internal static ApiResponse<dynamic> oauthExec (bool bEcho =true) {
			try {
				readKeys () ;
				TwoLeggedApi apiInstance =new TwoLeggedApi () ;
				ApiResponse<dynamic> bearer =apiInstance.AuthenticateWithHttpInfo (FORGE_CLIENT_ID, FORGE_CLIENT_SECRET, grantType, scope) ;
				if ( httpErrorHandler (bearer, "Failed to get your token", false) ) {
					System.IO.File.Delete ("data/access_token") ;
					return (null) ;
				}
				//Console.WriteLine (bearer.Data.ToString () as string) ;
				string token =bearer.Data.token_type + " " + bearer.Data.access_token ;
				if ( bEcho )
					Console.WriteLine ("Your new 2-legged access token is: " + token) ;
				DateTime dt =DateTime.Now ;
				dt.AddSeconds (double.Parse (bearer.Data.expires_in.ToString ())) ;
				if ( bEcho )
					Console.WriteLine ("Expires at: " + dt.ToLocalTime ()) ;
				System.IO.File.WriteAllText ("data/access_token", token) ;
				return (bearer) ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling TwoLeggedApi.AuthenticateWithHttpInfo: " + ex.Message) ;
				return (null) ;
			}
		}

		internal static string access_token (bool bStripType =true) {
			string st =System.IO.File.ReadAllText ("data/access_token") ;
			if ( bStripType )
				st =st.Split (new char [] { ' ' }) [1] ;
			return (st) ;
		}

		internal static string readOnlyAccessToken () {
			try {
				readKeys () ;
				TwoLeggedApi apiInstance =new TwoLeggedApi () ;
				ApiResponse<dynamic> bearer =apiInstance.AuthenticateWithHttpInfo (FORGE_CLIENT_ID, FORGE_CLIENT_SECRET, grantType, scopeViewer) ;
				httpErrorHandler (bearer, "Failed to get your token") ;
				//Console.WriteLine (bearer.Data.ToString () as string) ;
				return (bearer.Data.access_token) ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling TwoLeggedApi.AuthenticateWithHttpInfo: " + ex.Message) ;
				return (null) ;
			}
		}

		#endregion

		#region Commands
		internal static int RunTwoLegged (TwoLeggedOptions opts) {
			ApiResponse<dynamic> bearer =oauthExec () ;
			return (0) ;
		}

		internal static int RunThreeLegged (ThreeLeggedOptions opts) {
			ThreeLeggedApi oa3Legged =new ThreeLeggedApi () ;
			readKeys () ;
			if ( string.IsNullOrEmpty (opts.code) ) {
				string uri =oa3Legged.Authorize (FORGE_CLIENT_ID, "code", FORGE_CALLBACK, scope) ;
				System.Diagnostics.Process.Start (uri) ;
				Console.WriteLine ("Wait for the browser to return a code and run this script again with the code as parameter...") ;
				return (0) ;
			} else {
				try {
					ApiResponse<dynamic> bearer =oa3Legged.GettokenWithHttpInfo (FORGE_CLIENT_ID, FORGE_CLIENT_SECRET, "authorization_code", opts.code, FORGE_CALLBACK) ;
					if ( httpErrorHandler (bearer, "Failed to get your token", false) ) {
						System.IO.File.Delete ("data/access_token") ;
						return (2) ;
					}
					//Console.WriteLine (bearer.Data.ToString () as string) ;
					string token =bearer.Data.token_type + " " + bearer.Data.access_token ;
					Console.WriteLine ("Your new 3-legged access token is: " + token) ;
					DateTime dt =DateTime.Now ;
					dt.AddSeconds (double.Parse (bearer.Data.expires_in.ToString ())) ;
					Console.WriteLine ("Expires at: " + dt.ToLocalTime ()) ;
					System.IO.File.WriteAllText ("data/access_token", token) ;
					token =bearer.Data.token_type + " " + bearer.Data.refresh_token ;
					System.IO.File.WriteAllText ("data/refresh_token", token) ;
				} catch ( Exception ex ) {
					Console.Error.WriteLine ("Exception when calling ThreeLeggedApi.Gettoken: " + ex.Message) ;
					return (1) ;
				}			
			}
			return (0) ;
		}

		internal static int RunAboutMe (AboutMeOptions opts) {
			try {
				Console.WriteLine ("About Me!...") ;
				InformationalApi oa3Info =new InformationalApi () ;
				oa3Info.Configuration.AccessToken =access_token () ;
				ApiResponse<dynamic> response =oa3Info.AboutMeWithHttpInfo () ;
				httpErrorHandler (response, "Failed to access user information") ;
				Console.WriteLine (response.Data.ToString () as string) ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling InformationalApi.AboutMeWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}

		internal static int RunBuckets (BucketsOptions opts) {
			if ( opts.server ) {
				Console.WriteLine ("Listing from " + opts.startAt + " to " + opts.limit) ;
				try {
					BucketsApi ossBuckets =new BucketsApi () ;
					ossBuckets.Configuration.AccessToken =access_token () ;
					ApiResponse<dynamic> response =ossBuckets.GetBucketsWithHttpInfo (opts.region, opts.limit, opts.startAt) ;
					httpErrorHandler (response, "Failed to access buckets list") ;
					Console.WriteLine (response.Data.ToString () as string) ;
					if ( !hasOwnProperty (response.Data, "next") ) {
						Console.WriteLine ("Your search is complete, no more items to list") ;
					} else {
						var uri =new Uri (response.Data.next) ;
						NameValueCollection url_parts =System.Web.HttpUtility.ParseQueryString (uri.Query) ;
						string startAt =url_parts ["startAt"] ;
						Console.WriteLine ("Your next search startAt is: " + startAt) ;
					}
				} catch ( Exception ex ) {
					Console.Error.WriteLine ("Exception when calling BucketsApi.GetBucketsWithHttpInfo: " + ex.Message) ;
					return (2) ;
				}
			} else {
				string[] files =System.IO.Directory.GetFiles ("data", "*.bucket.json") ;
				if ( files.Length == 0 )
					Console.WriteLine ("0 file in folder 'data'") ;
				foreach ( string fn in files )
					Console.WriteLine (fn) ;
			}
			return (0) ;
		}

		internal static int RunBucket (BucketOptions opts) {
			if ( string.IsNullOrEmpty (opts.bucket) ) {
				string data =System.IO.File.ReadAllText ("data/bucket") ;
				Console.Write ("Current bucket is: " + data) ;
			} else {
				if ( !checkBucketKey (opts.bucket) )
					return (3) ;
				System.IO.File.WriteAllText ("data/bucket", opts.bucket) ;
				Console.Write ("Your current bucket is now: " + opts.bucket) ;
			}
			return (0) ;
		}

		internal static int RunBucketCreate (BucketCreateOptions opts) {
			if ( !checkBucketKey (opts.bucket) )
				return (2) ;
			try {
				Console.WriteLine ("Create bucket: " + opts.bucket) ;
				BucketsApi ossBuckets =new BucketsApi () ;
				ossBuckets.Configuration.AccessToken =access_token () ;
				PostBucketsPayload payload =new PostBucketsPayload (opts.bucket, null, opts.type) ;
				ApiResponse<dynamic> response =ossBuckets.CreateBucketWithHttpInfo (payload, opts.region) ;
				httpErrorHandler (response, "Failed to create bucket") ;
				System.IO.File.WriteAllText ("data/bucket", opts.bucket) ;
				System.IO.File.WriteAllText ("data/" + opts.bucket + ".bucket.json", response.Data.ToString () as string) ;
				Console.WriteLine ("bucket created") ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling BucketsApi.CreateBucketWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}

		internal static int RunBucketCheck (BucketCheckOptions opts) {
			string bucketKey =currentBucket (opts.bucket) ;
			if ( !checkBucketKey (bucketKey) )
				return (2) ;
			try {
				Console.WriteLine ("Getting bucket details") ;
				BucketsApi ossBuckets =new BucketsApi () ;
				ossBuckets.Configuration.AccessToken =access_token () ;
				ApiResponse<dynamic> response =ossBuckets.GetBucketDetailsWithHttpInfo (bucketKey) ;
				httpErrorHandler (response, "Failed to access bucket details") ;
				if ( response.Data.policyKey == "transient" ) // 24 hours
					Console.WriteLine ("bucket content will expire after: 24 hours") ;
				else if ( response.Data.policyKey == "temporary" ) // 30 days
					Console.WriteLine ("bucket content will expire after: 30 days") ;
				else // persistent
					Console.WriteLine ("bucket content will never expire") ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling BucketsApi.GetBucketDetailsWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}

		internal static int RunBucketItems (BucketItemsOptions opts) {
			string bucketKey =currentBucket (opts.bucket) ;
			if ( !checkBucketKey (bucketKey) )
				return (2) ;
			try {
				Console.WriteLine ("Listing bucket " + bucketKey + " content") ;
				ObjectsApi ossObjects =new ObjectsApi () ;
				ossObjects.Configuration.AccessToken =access_token () ;
				ApiResponse<dynamic> response =ossObjects.GetObjectsWithHttpInfo (bucketKey, opts.limit, null, opts.startAt) ;
				httpErrorHandler (response, "Failed to access buckets list") ;
				Console.WriteLine (response.Data.ToString () as string) ;
				if ( !hasOwnProperty (response.Data, "next") ) {
					Console.WriteLine ("Your search is complete, no more items to list") ;
				} else {
					var uri =new Uri (response.Data.next) ;
					NameValueCollection url_parts =System.Web.HttpUtility.ParseQueryString (uri.Query) ;
					string startAt =url_parts ["startAt"] ;
					Console.WriteLine ("Your next search startAt is: " + startAt) ;
				}
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling ObjectsApi.GetObjectsWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}

		internal static int RunBucketDelete (BucketDeleteOptions opts) {
			string bucketKey =currentBucket (opts.bucket) ;
			if ( !checkBucketKey (bucketKey) )
				return (2) ;
			try {
				Console.WriteLine ("Delete bucket: " + bucketKey) ;
				BucketsApi ossBuckets =new BucketsApi () ;
				ossBuckets.Configuration.AccessToken =access_token () ;
				ApiResponse<dynamic> response =ossBuckets.DeleteBucketWithHttpInfo (bucketKey) ;
                httpErrorHandler (response, "Failed to delete bucket", false) ;
				System.IO.File.Delete ("data/bucket") ;
                System.IO.File.Delete ("data/" + bucketKey + ".bucket.json") ;
				Console.WriteLine ("bucket deleted") ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling BucketsApi.DeleteBucketWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}

		internal static int RunUpload (UploadOptions opts) {
			string bucketKey =currentBucket (string.Empty) ;
			if ( !checkBucketKey (bucketKey) )
				return (2) ;
			try {
				string fileKey =System.IO.Path.GetFileName (opts.filename) ;
				if ( !System.IO.File.Exists (opts.filename) ) {
					Console.WriteLine (opts.filename + " does not exists (or not found)") ;
					return (2) ;
				}
				ObjectsApi ossObjects =new ObjectsApi () ;
				ossObjects.Configuration.AccessToken =access_token () ;
				Console.WriteLine ("Uploading file: " + opts.filename) ;
				using ( StreamReader streamReader =new StreamReader (opts.filename) ) {
					ApiResponse<dynamic> response =ossObjects.UploadObjectWithHttpInfo (bucketKey,
						fileKey, (int)streamReader.BaseStream.Length, streamReader.BaseStream,
						"application/octet-stream") ;
					httpErrorHandler (response, "Failed to upload file") ;
					System.IO.File.WriteAllText ("data/" + bucketKey + "." + fileKey + ".json", response.Data.ToString () as string) ;
					Console.WriteLine ("Upload successful") ;
					Console.WriteLine ("ID: " + response.Data.objectId) ;
					Console.WriteLine ("URN: " + Base64Encode (response.Data.objectId)) ;
					Console.WriteLine ("Location: " + response.Data.location) ;
				}
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling ObjectsApi.UploadObject: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}

		internal static int RunResumable (ResumableOptions opts) {
			string bucketKey =currentBucket (string.Empty) ;
			if ( !checkBucketKey (bucketKey) )
				return (2) ;
			try {
				string fileKey =System.IO.Path.GetFileName (opts.filename) ;
				if ( !System.IO.File.Exists (opts.filename) ) {
					Console.WriteLine (opts.filename + " does not exists (or not found)") ;
					return (2) ;
				}
				long size =new System.IO.FileInfo (opts.filename).Length ;
				long pieceSz =size / opts.pieces ;
				long modSz=size % opts.pieces ;
				if ( modSz > 0 )
					opts.pieces++ ;
				Console.WriteLine ("Uploading file: " + Path.GetFileName (opts.filename) + " in " + opts.pieces + " pieces") ;
				using ( FileStream streamReader =new FileStream (opts.filename, FileMode.Open) ) {
					string sessionId =RandomString (12) ;
					ObjectsApi ossObjects =new ObjectsApi () ;
					for ( int i =0 ; i < opts.pieces ; i++ ) {
						long start =i * pieceSz ;
						long end =Math.Min (size, (i + 1) * pieceSz) - 1 ;
						string range ="bytes " + start + "-" + end + "/" + size ;
						long length =end - start + 1 ;
						Console.WriteLine ("Loading " + range) ;
						byte [] buffer =new byte [length] ;
						MemoryStream memoryStream =new MemoryStream (buffer) ;
						int nb =streamReader.Read (buffer, 0, (int)length) ;
						//memoryStream.Seek (0, SeekOrigin.Begin) ;
						memoryStream.Write (buffer, 0, nb) ;
						memoryStream.Position =0 ;
						ApiResponse<dynamic> bearer =oauthExec (false) ;
						ossObjects.Configuration.AccessToken =bearer.Data.access_token ;
						ApiResponse<dynamic> response =ossObjects.UploadChunkWithHttpInfo (bucketKey,
							fileKey, (int)length, range, sessionId, memoryStream,
							"application/octet-stream") ;
						httpErrorHandler (response, "Failed to upload partial file") ;
						if ( response.StatusCode == 202 ) {
							Console.WriteLine ("Partial upload accepted") ;
							continue ;
						}
						System.IO.File.WriteAllText ("data/" + bucketKey + "." + fileKey + ".json", response.Data.ToString () as string) ;
						Console.WriteLine ("Upload successful") ;
						Console.WriteLine ("ID: " + response.Data.objectId) ;
						Console.WriteLine ("URN: " + Base64Encode (response.Data.objectId)) ;
						Console.WriteLine ("Location: " + response.Data.location) ;
					}
				}
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling ObjectsApi.UploadChunkWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}

		internal static int RunDownload (DownloadOptions opts) {
			string bucketKey =currentBucket (string.Empty) ;
			if ( !checkBucketKey (bucketKey) )
				return (2) ;
			try {
				if ( opts.file )
					opts.fileKey =readFileKey (bucketKey, opts.fileKey) ;
				Console.WriteLine ("Downloading file: " + opts.fileKey) ;
				ObjectsApi ossObjects =new ObjectsApi () ;
				ossObjects.Configuration.AccessToken =access_token () ;
				ApiResponse<dynamic> response =ossObjects.GetObjectWithHttpInfo (bucketKey, opts.fileKey) ;
				httpErrorHandler (response, "Failed to download file") ;
				Stream downloadObj =response.Data as Stream ;
				downloadObj.Position =0 ;
				//using ( BinaryReader sr =new BinaryReader (downloadObj) )
				using ( FileStream outputFile =new FileStream (opts.outputFile, FileMode.Create) )
					downloadObj.CopyTo (outputFile) ;
				Console.WriteLine ("Download successful") ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling ObjectsApi.GetObjectWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}
		
		internal static int RunObjectDetails (ObjectDetailsOptions opts) {
			string bucketKey =currentBucket (string.Empty) ;
			if ( !checkBucketKey (bucketKey) )
				return (2) ;
			try {
				if ( opts.file )
					opts.fileKey =readFileKey (bucketKey, opts.fileKey) ;
				Console.WriteLine ("Getting details for file: " + opts.fileKey) ;
				ObjectsApi ossObjects =new ObjectsApi () ;
				ossObjects.Configuration.AccessToken =access_token () ;
				ApiResponse<dynamic> response =ossObjects.GetObjectDetailsWithHttpInfo (bucketKey, opts.fileKey) ;
				httpErrorHandler (response, "Failed to download file") ;
				Console.WriteLine (response.Data.ToString () as string) ;
				System.IO.File.WriteAllText ("data/" + bucketKey + "." + opts.fileKey + ".json", response.Data.ToString () as string) ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling ObjectsApi.GetObjectDetailsWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}

		internal static int RunTranslate (TranslateOptions opts) {
			var bucketKey =currentBucket (string.Empty) ;
			if ( !checkBucketKey (bucketKey) )
				return (2) ;
			try {
				if ( opts.file )
					opts.fileKey =readFileKey (bucketKey, opts.fileKey) ;
				Console.WriteLine ("Request file to be translated") ;
				string urn =URN (bucketKey, opts.fileKey, false) ;
				if ( opts.entry == null )
					opts.entry =opts.fileKey ;
				JobPayloadInput jobInput =new JobPayloadInput (
					urn,
					Path.GetExtension (opts.fileKey).ToLower () == ".zip",
					opts.entry
				) ;
				JobPayloadOutput jobOutput =new JobPayloadOutput (
					new List<JobPayloadItem> (
						new JobPayloadItem [] {
							new JobPayloadItem (
								JobPayloadItem.TypeEnum.Svf,
								new List<JobPayloadItem.ViewsEnum> (
									new JobPayloadItem.ViewsEnum [] {
										JobPayloadItem.ViewsEnum._2d, JobPayloadItem.ViewsEnum._3d }
								),
								null
							)}
					)) ;
				JobPayload job =new JobPayload (jobInput, jobOutput) ;
				bool bForce =true ;
				DerivativesApi md =new DerivativesApi () ;
				md.Configuration.AccessToken =access_token () ;
				ApiResponse<dynamic> response =md.TranslateWithHttpInfo (job, bForce) ;
				httpErrorHandler (response, "Failed to register file for translation") ;
				Console.WriteLine ("Registration successfully submitted.") ;
				Console.WriteLine (response.Data.ToString () as string) ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling DerivativesApi.TranslateWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}

		internal static int RunTranslateProgress (TranslateProgressOptions opts) {
			string bucketKey =currentBucket (string.Empty) ;
			if ( !checkBucketKey (bucketKey) )
				return (2) ;
			try {
				if ( opts.file )
					opts.fileKey =readFileKey (bucketKey, opts.fileKey) ;
				Console.WriteLine ("Checking file translation progress") ;
				string urn =URN (bucketKey, opts.fileKey) ;
				DerivativesApi md =new DerivativesApi () ;
				md.Configuration.AccessToken =access_token () ;
				ApiResponse<dynamic> response =md.GetManifestWithHttpInfo (urn) ;
				httpErrorHandler (response, "Failed to get file manifest") ;
				Console.WriteLine ("Request: " + response.Data.status + " (" + response.Data.progress + ")") ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling DerivativesApi.GetManifestWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}

		internal static int RunManifest (ManifestOptions opts) {
			string bucketKey =currentBucket (string.Empty) ;
			if ( !checkBucketKey (bucketKey) )
				return (2) ;
			try {
				if ( opts.file )
					opts.fileKey =readFileKey (bucketKey, opts.fileKey) ;
				Console.WriteLine ("Getting file manifest") ;
				string urn =URN (bucketKey, opts.fileKey) ;
				DerivativesApi md =new DerivativesApi () ;
				md.Configuration.AccessToken =access_token () ;
				ApiResponse<dynamic> response =md.GetManifestWithHttpInfo (urn) ;
				httpErrorHandler (response, "Failed to get file manifest") ;
				Console.WriteLine (response.Data.ToString () as string) ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling DerivativesApi.GetManifestWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}

		internal static int RunMetadata (MetadataOptions opts) {
			string bucketKey =currentBucket (string.Empty) ;
			if ( !checkBucketKey (bucketKey) )
				return (2) ;
			try {
				if ( opts.file )
					opts.fileKey =readFileKey (bucketKey, opts.fileKey) ;
				Console.WriteLine ("Getting file metadata") ;
				string urn =URN (bucketKey, opts.fileKey) ;
				DerivativesApi md =new DerivativesApi () ;
				md.Configuration.AccessToken =access_token () ;
				ApiResponse<dynamic> response =md.GetMetadataWithHttpInfo (urn) ;
				httpErrorHandler (response, "Failed to get file metadata") ;
				Console.WriteLine (response.Data.ToString () as string) ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling DerivativesApi.GetMetadataWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}

		internal static int RunThumbnail (ThumbnailOptions opts) {
			string bucketKey =currentBucket (string.Empty) ;
			if ( !checkBucketKey (bucketKey) )
				return (2) ;
			try {
				if ( opts.file )
					opts.fileKey =readFileKey (bucketKey, opts.fileKey) ;
				Console.WriteLine ("Getting file thumbnail") ;
				string urn =URN (bucketKey, opts.fileKey) ;
				DerivativesApi md =new DerivativesApi () ;
				md.Configuration.AccessToken =access_token () ;
				ApiResponse<dynamic> response =md.GetThumbnailWithHttpInfo (urn, 400, 400) ;
				httpErrorHandler (response, "Failed to get file thumbnail") ;
				Stream downloadObj =response.Data as Stream ;
				downloadObj.Position =0 ;
				//using ( BinaryReader sr =new BinaryReader (downloadObj) )
				using ( FileStream outputFile =new FileStream (opts.outputFile, FileMode.Create) )
					downloadObj.CopyTo (outputFile) ;
				Console.WriteLine ("Thumbnail downloaded successfully") ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling DerivativesApi.GetThumbnailWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}

		internal static int RunDeleteFile (DeleteFileOptions opts) {
			string bucketKey =currentBucket (string.Empty) ;
			if ( !checkBucketKey (bucketKey) )
				return (2) ;
			try {
				if ( opts.file )
					opts.fileKey =readFileKey (bucketKey, opts.fileKey) ;
				Console.WriteLine ("Deleting file: " + opts.fileKey) ;
				ObjectsApi ossObjects =new ObjectsApi () ;
				ossObjects.Configuration.AccessToken =access_token () ;
				ApiResponse<dynamic> response =ossObjects.DeleteObjectWithHttpInfo (bucketKey, opts.fileKey) ;
				httpErrorHandler (response, "Failed to delete file") ;
				Console.WriteLine ("File deleted") ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling ObjectsApi.DeleteObjectWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}
		
		internal static int RunDeleteManifest (DeleteManifestOptions opts) {
			string bucketKey =currentBucket (string.Empty) ;
			if ( !checkBucketKey (bucketKey) )
				return (2) ;
			try {
				if ( opts.file )
					opts.fileKey =readFileKey (bucketKey, opts.fileKey) ;
				Console.WriteLine ("Deleting manifest for " + opts.fileKey) ;
				string urn =URN (bucketKey, opts.fileKey) ;
				DerivativesApi md =new DerivativesApi () ;
				md.Configuration.AccessToken =access_token () ;
				ApiResponse<dynamic> response =md.DeleteManifestWithHttpInfo (urn) ;
				httpErrorHandler (response, "Failed to delete manifest") ;
				Console.WriteLine ("Manifest deleted") ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling DerivativesApi.DeleteManifestWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}

		internal static int RunHtml (HtmlOptions opts) {
			string bucketKey =currentBucket (string.Empty) ;
			if ( !checkBucketKey (bucketKey) )
				return (2) ;
			try {
				if ( opts.file )
					opts.fileKey =readFileKey (bucketKey, opts.fileKey) ;
				Console.WriteLine ("Creating file: " + opts.outputFile) ;
				string urn =URN (bucketKey, opts.fileKey) ;
				string accessToken =readOnlyAccessToken () ;
				string st =_html.Replace ("__URN__", urn).Replace ("__ACCESS_TOKEN__", accessToken) ;
				System.IO.File.WriteAllText (opts.outputFile, st) ;
				Console.WriteLine ("File created") ;
			} catch ( Exception ex ) {
				Console.Error.WriteLine ("Exception when calling DerivativesApi.DeleteManifestWithHttpInfo: " + ex.Message) ;
				return (2) ;
			}
			return (0) ;
		}

		#endregion

		#region Utils
		internal static bool httpErrorHandler (ApiResponse<dynamic> response, string msg ="", bool bExit =true) {
			if ( response.StatusCode < 200 || response.StatusCode >= 300 ) {
				Console.Error.WriteLine (msg) ;
				Console.Error.WriteLine ("HTTP " + response.StatusCode) ;
				if ( bExit )
					System.Environment.Exit (1) ;
				return (true) ;
			}
			return (false) ;
		}

		public static bool hasOwnProperty (dynamic obj, string name) {
			try {
				var test =obj [name] ;
				return (true) ;
			} catch ( Exception ) {
				return (false) ;
			}
		}

		private static bool checkBucketKey (string name) {
			Regex rgx =new Regex (@"^[-_.a-z0-9]{3,128}$") ;
			bool result =rgx.IsMatch (name) ;
			if ( !result )
				Console.Error.WriteLine ("Invalid bucket name") ;
			return (result) ;
		}

		internal static string currentBucket (string name) {
			if ( string.IsNullOrEmpty (name) )
				return (System.IO.File.ReadAllText ("data/bucket")) ;
			return (name) ;
		}

		internal static string readFileKey (string bucketKey, string fileKey) {
			try {
				string details =System.IO.File.ReadAllText ("data/" + bucketKey + "." + fileKey + ".json") ;
				JObject json =JObject.Parse (details) ;
				return (json ["objectKey"].ToString ()) ;
			} catch ( Exception ) {
				return ("") ;
			}
		}

		internal static string URN (string bucketKey, string fileKey, bool bSafe =true) {
			string urn ="urn:adsk.objects:os.object:" + bucketKey + "/" + fileKey ;
			try {
				string details =System.IO.File.ReadAllText ("data/" + bucketKey + "." + fileKey + ".json") ;
				JObject json =JObject.Parse (details) ;
				urn =bSafe ? SafeBase64Encode (json ["objectId"].ToString ()) : Base64Encode (json ["objectId"].ToString ()) ;
			} catch ( Exception ) {
			}
			return (urn) ;
		}

		private static Random random =new Random() ;
		public static string RandomString (int length =8) {
			const string chars ="ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" ;
			return (new string (Enumerable.Repeat (chars, length)
				.Select (s => s [random.Next (s.Length)]).ToArray ())
			) ;
		}

		public static string Base64Encode (string plainText) {
			var plainTextBytes =System.Text.Encoding.UTF8.GetBytes (plainText) ;
			return (System.Convert.ToBase64String (plainTextBytes)) ;
		}

		public static string Base64Decode (string base64EncodedData) {
			var base64EncodedBytes =System.Convert.FromBase64String (base64EncodedData) ;
			return (System.Text.Encoding.UTF8.GetString (base64EncodedBytes)) ;
		}

		private static readonly char [] padding ={ '=' } ;
		public static string SafeBase64Encode (string plainText) {
			var plainTextBytes =System.Text.Encoding.UTF8.GetBytes (plainText) ;
			return (System.Convert.ToBase64String (plainTextBytes)
				.TrimEnd (padding).Replace ('+', '-').Replace ('/', '_')
			) ;
		}

		public static string SafeBase64Decode (string base64EncodedData) {
			string st =base64EncodedData.Replace ('_', '/').Replace ('-', '+') ;
			switch ( base64EncodedData.Length % 4 ) {
				case 2: st +="==" ; break ;
				case 3: st +="=" ; break ;
			}
			var base64EncodedBytes =System.Convert.FromBase64String (st) ;
			return (System.Text.Encoding.UTF8.GetString (base64EncodedBytes)) ;
		}

		#endregion

		#region Html
		private static readonly string _html =@"<!DOCTYPE html>
<html>
<head>
	<meta charset=""UTF-8"">
	<script src=""https://developer.api.autodesk.com/viewingservice/v1/viewers/three.min.css""></script>
	<link rel=""stylesheet"" href=""https://developer.api.autodesk.com/viewingservice/v1/viewers/style.min.css"" />
	<script src=""https://developer.api.autodesk.com/viewingservice/v1/viewers/viewer3D.min.js""></script>
</head>
<body onload=""initialize()"">
<div id=""viewer"" style=""position:absolute; width:90%; height:90%;""></div>
<script>
	function authMe () { return ('__ACCESS_TOKEN__') ; }

	function initialize () {
		var options ={
			'document' : ""urn:__URN__"",
			'env': 'AutodeskProduction',
			'getAccessToken': authMe
		} ;
		var viewerElement =document.getElementById ('viewer') ;
		//var viewer =new Autodesk.Viewing.Viewer3D (viewerElement, {}) ; / No toolbar
		var viewer =new Autodesk.Viewing.Private.GuiViewer3D (viewerElement, {}) ; // With toolbar
		Autodesk.Viewing.Initializer (options, function () {
			viewer.initialize () ;
			loadDocument (viewer, options.document) ;
		}) ;
	}
	function loadDocument (viewer, documentId) {
		// Find the first 3d geometry and load that.
		Autodesk.Viewing.Document.load (
			documentId,
			function (doc) { // onLoadCallback
				var geometryItems =[] ;
				geometryItems =Autodesk.Viewing.Document.getSubItemsWithProperties (
					doc.getRootItem (),
					{ 'type' : 'geometry', 'role' : '3d' },
					true
				) ;
				if ( geometryItems.length <= 0 ) {
					geometryItems =Autodesk.Viewing.Document.getSubItemsWithProperties (
						doc.getRootItem (),
						{ 'type': 'geometry', 'role': '2d' },
						true
					) ;
				}
				if ( geometryItems.length > 0 )
					viewer.load (
						doc.getViewablePath (geometryItems [0])//,
						//null, null, null,
						//doc.acmSessionId /*session for DM*/
					) ;
			},
			function (errorMsg) { // onErrorCallback
				alert(""Load Error: "" + errorMsg) ;
			}//,
			//{
            //	'oauth2AccessToken': authMee (),
            //	'x-ads-acm-namespace': 'WIPDM',
            //	'x-ads-acm-check-groups': 'true',
        	//}
		) ;
	}
</script>
</body>
</html>" ;

		#endregion

	}

}
