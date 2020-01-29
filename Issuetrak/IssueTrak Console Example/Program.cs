////////////////////////////////////////////////////////////////////////////////////////////////////
// Summary:	This example program demonstrates the essentials of initiating a connection to the
// IssueTrak API using the supplied Issuetrak.API.Client.
// 
// To run these samples successfully, the following configuration values need to be set in the app.config file
// as appSetting elements.
// 
// (1)  The endpoint URL for the deployed Issuetrak API, e.g., http://local.issuetrakapi.com
//      Example:  <add key="BaseAPIURL" value="http://local.issuetrakapi.com" />
// (2)  The API version number, e.g., 1
//      Example:  <add key="APIVersion" value="1" />
// (3)  The API key created using the Issuetrak.API.KeyTool.
//      Example:  <add key="APIKey" value="API_KEY_HERE" />
//  
//  Several of the samples will need further implementation in order to achieve successful responses.
//  For example, the await CreateIssueAsync(new CreateIssueDTO()) example call requires that the
//  desired CreateIssueDTO properties be filled with valid values.
//  
//  If the await CreateIssueAsync(new CreateIssueDTO()) is run without filling these property values,
//  an HTTP response error code will be displayed.
////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Issuetrak.DataModels.DTOs;
using Newtonsoft.Json;
using Issuetrak.API.Data.Models.Models.OrganizationModels;

namespace Issuetrak.API.Client.Example
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Provides an example of how to consume the Issuetrak API using the Issuetrak .NET client.
    /// </summary>
    ///
    /// <remarks>   09/15/2014. </remarks>
    ///
    /// <seealso cref="T:System.IDisposable"/>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    internal class Program : IDisposable
    {
        #region Private Constants

        /// <summary>   Identifier for the test cause. </summary>
        private const int _testCauseID = 5;

        /// <summary>   Identifier for the test class. </summary>
        private const int _testClassID = 1;

        /// <summary>   The test issue number. </summary>
        private const int _testIssueNumber = 1;

        /// <summary>   Identifier for the test attachment. </summary>
        private const int _testAttachmentID = 26;

        /// <summary>   Identifier for the test note. </summary>
        private const int _testNoteID = 1;

        /// <summary>   Identifier for the test department. </summary>
        private const int _testDepartmentID = 1;

        /// <summary>   Identifier for the test issueType. </summary>
        private const int _testIssuetypeID = 5;

        /// <summary>   Identifier for the test issueSubType. </summary>
        private const int _testIssuesubtypeID = 21;

        /// <summary>   Identifier for the test issueSubType2. </summary>
        private const int _testIssuesubtype2ID = 3;

        /// <summary>   Identifier for the test issueSubType3. </summary>
        private const int _testIssuesubtype3ID = 3;

        /// <summary>   Identifier for the test issueSubType4. </summary>
        private const int _testIssuesubtype4ID = 3;

        /// <summary>   Identifier for the test location. </summary>
        private const string _testLocationID = "HQ";

        /// <summary>   Identifier for the test location. </summary>
        private const string _testNewLocationID = "HQ-2";

        /// <summary>   Identifier for the test menu item. </summary>
        private const int _testMenuItemID = 1;

        /// <summary>   Identifier for the test organization. </summary>
        private const int _testOrganizationID = 106;

        /// <summary>   Identifier for the test priority. </summary>
        private const int _testPriorityID = 1;

        /// <summary>   Identifier for the test project. </summary>
        private const int _testProjectID = 1;

        /// <summary>   Identifier for the test service level. </summary>
        private const int _testServiceLevelID = 1;

        /// <summary>   Identifier for the test service level agreement. </summary>
        private const int _testServiceLevelAgreementID = 1;

        /// <summary>   Identifier for the test service level severity. </summary>
        private const int _testServiceLevelSeverityID = 1;

        /// <summary>   Identifier for the test service level term. </summary>
        private const int _testServiceLevelTermID = 1;

        /// <summary>   Identifier for the test substatus. </summary>
        private const int _testSubstatusID = 13;

        /// <summary>   Identifier for the test timezone. </summary>
        private const int _testTimezoneID = 26;
        
        /// <summary>   Identifier for the test user type. </summary>
        private const int _testUserTypeID = 1;

        /// <summary>  Identifier for the end user type.</summary>
        private const int _testEndUserTypeID = 2;

        /// <summary>   Identifier for the test user. </summary>
        private const string _testUserID = "APIUser";

        /// <summary>   Identifier for the test update user. </summary>
        private const string _testUpdateUserID = "InactiveUser";

        /// <summary>   Numeric identifier for the test user. </summary>
        private const int _testUpdateUserNumber = 14;

        /// <summary>   Identifier for the test user-defined field type. </summary>
        private const int _testUserdefinedfieldtypeID = 1;

        /// <summary>   The valid list format. </summary>
        private const string _testListFormat = "Standard";

        /// <summary>   URL of the valid redirect. </summary>
        private const string _testRedirectURL = "CSIssue_Submit.asp";

        private const string _testOrganizationName = "My Organization";

        #endregion

        #region Private Static Fields

        /// <summary>   Represents a large attachment file. </summary>
        private static readonly string _longTextBlock = new string('A', 5000000);

        /// <summary>   Represents the bytes for the large attachment file contents. </summary>
        private static readonly byte[] _longTextBlockBytes = Encoding.ASCII.GetBytes(_longTextBlock);

        /// <summary>   The test method definitions available for testing. </summary>
        private static readonly List<Tuple<string, Func<Task>>> _testMethodDefinitions = new List
            <Tuple<string, Func<Task>>>
        {
            // Methods to test Attachment-related functionality:
            new Tuple<string, Func<Task>>("CreateAttachmentAsync",
                                          () => CreateAttachmentAsync(new CreateAttachmentDTO
                                          {
                                              CreatedBy = "Admin",
                                              CreatedDate = DateTime.Now,
                                              FileName = "Test-Attachment-File.txt",
                                              FileContent = _longTextBlockBytes,
                                              FileSizeInBytes = _longTextBlockBytes.Length,
                                              IssueNumber = _testIssueNumber
                                          })),

            new Tuple<string, Func<Task>>("GetAttachmentForAttachmentIDAsync",
                                          () => GetAttachmentForAttachmentIDAsync(_testAttachmentID)),
            new Tuple<string, Func<Task>>("GetAttachmentsForIssueNumberAsync",
                                          () => GetAttachmentsForIssueNumberAsync(_testIssueNumber)),
            new Tuple<string, Func<Task>>("GetAttachmentsInCompressedArchiveForIssueNumberAsync",
                                          () => GetAttachmentsInCompressedArchiveForIssueNumberAsync(_testIssueNumber)),

            // Methods to test Cause-related functionality:
            new Tuple<string, Func<Task>>("GetCauseForCauseIDAsync",
                                          () => GetCauseForCauseIDAsync(_testCauseID)),
            new Tuple<string, Func<Task>>("GetAllCausesAsync", GetAllCausesAsync),

            // Methods to test Class-related functionality:
            new Tuple<string, Func<Task>>("GetClassForClassIDAsync",
                                          () => GetClassForClassIDAsync(_testClassID)),
            new Tuple<string, Func<Task>>("GetAllClassesAsync", GetAllClassesAsync),

            // Methods to test Department-related functionality:
            new Tuple<string, Func<Task>>("GetDepartmentForDepartmentIDAsync",
                                          () => GetDepartmentForDepartmentIDAsync(_testDepartmentID)),
            new Tuple<string, Func<Task>>("GetAllDepartmentsAsync", GetAllDepartmentsAsync),



            // Methods to test Issue-related functionality:
            new Tuple<string, Func<Task>>("CreateIssueAsync", () => CreateIssueAsync(GenerateTestCreateIssueDTOInstance())),
            new Tuple<string, Func<Task>>("UpdateIssueAsync", () => UpdateIssueAsync(GenerateTestUpdateIssueDTOInstance())),
            new Tuple<string, Func<Task>>("GetIssueForIssueNumberAsync",
                                          () => GetIssueForIssueNumberAsync(_testIssueNumber, true)),
            new Tuple<string, Func<Task>>("GetIssuesForIssueNumberListAsync",
                                          () => GetIssuesForIssueNumberListAsync(new[] {_testIssueNumber}, true)),
            new Tuple<string, Func<Task>>("SearchIssuesAsync",
                                          () => SearchIssuesAsync(GenerateTestSearchIssueDTOInstance())),

            // Methods to test IssueType-related functionality:
            new Tuple<string, Func<Task>>("GetIssueTypeForIssueTypeIDAsync",
                                          () => GetIssueTypeForIssueTypeIDAsync(_testIssuetypeID)),
            new Tuple<string, Func<Task>>("GetAllIssueTypesAsync", GetAllIssueTypesAsync),

            // Methods to test IssueSubType-related functionality:
            new Tuple<string, Func<Task>>("GetIssueSubTypeForIssueSubTypeIDAsync",
                                          () => GetIssueSubTypeForIssueSubTypeIDAsync(_testIssuesubtypeID)),
            new Tuple<string, Func<Task>>("GetAllIssueSubTypesAsync", GetAllIssueSubTypesAsync),

            // Methods to test IssueSubType2-related functionality:
            new Tuple<string, Func<Task>>("GetIssueSubType2ForIssueSubType2IDAsync",
                                          () => GetIssueSubType2ForIssueSubType2IDAsync(_testIssuesubtype2ID)),
            new Tuple<string, Func<Task>>("GetAllIssueSubTypes2Async", GetAllIssueSubTypes2Async),

            // Methods to test IssueSubType3-related functionality:
            new Tuple<string, Func<Task>>("GetIssueSubType3ForIssueSubType3IDAsync",
                                          () => GetIssueSubType3ForIssueSubType3IDAsync(_testIssuesubtype3ID)),
            new Tuple<string, Func<Task>>("GetAllIssueSubTypes3Async", GetAllIssueSubTypes3Async),

            // Methods to test IssueSubType4-related functionality:
            new Tuple<string, Func<Task>>("GetIssueSubType4ForIssueSubType4IDAsync",
                                          () => GetIssueSubType4ForIssueSubType4IDAsync(_testIssuesubtype4ID)),
            new Tuple<string, Func<Task>>("GetAllIssueSubTypes4Async", GetAllIssueSubTypes4Async),

            // Methods to test Location-related functionality:
            new Tuple<string, Func<Task>>("GetLocationForLocationIDAsync",
                                          () => GetLocationForLocationIDAsync(_testLocationID)),
            new Tuple<string, Func<Task>>("GetAllLocationsAsync", GetAllLocationsAsync),
            new Tuple<string, Func<Task>>("CreateLocationsAsync", () => CreateLocationsAsync(new CreateLocationDTO
            {
                LocationID = _testNewLocationID,
                LocationName = _testNewLocationID

            })),
            new Tuple<string, Func<Task>>("UpdateLocationsAsync", () => UpdateLocationsAsync(new UpdateLocationDTO
            {
                LocationID = _testLocationID,
                LocationName = _testLocationID
            })),

            // Methods to test MenuItem-related functionality:
            new Tuple<string, Func<Task>>("GetMenuItemForMenuItemIDAsync",
                                          () => GetMenuItemForMenuItemIDAsync(_testMenuItemID)),
            new Tuple<string, Func<Task>>("GetAllMenuItemsAsync", GetAllMenuItemsAsync),

                
            // Methods to test Note-related functionality:
            new Tuple<string, Func<Task>>("CreateNoteAsync", () => CreateNoteAsync(GenerateTestCreateNoteDTOInstance())),
            new Tuple<string, Func<Task>>("GetNoteForNoteIDAsync", () => GetNoteForNoteIDAsync(_testNoteID)),
            new Tuple<string, Func<Task>>("GetNotesForIssueNumberAsync",
                                          () => GetNotesForIssueNumberAsync(_testIssueNumber)),

            // Methods to test Organization-related functionality:
            new Tuple<string, Func<Task>>("GetOrganizationForOrganizationIDAsync",
                                          () => GetOrganizationForOrganizationIDAsync(_testOrganizationID)),
            new Tuple<string, Func<Task>>("GetAllOrganizationsAsync", GetAllOrganizationsAsync),
            new Tuple<string, Func<Task>>("CreateOrganizationAsync", () => CreateOrganizationAsync(new CreateOrganizationDTO
            {
                OrganizationName = _testOrganizationName
            })),
            new Tuple<string, Func<Task>>("UpdateOrganizationAsync", () => UpdateOrganizationAsync(new UpdateOrganizationDTO
            {
                OrganizationID = _testOrganizationID,
                OrganizationName = _testOrganizationName
            })),


            // Methods to test Priority-related functionality:
            new Tuple<string, Func<Task>>("GetPriorityForPriorityIDAsync",
                                          () => GetPriorityForPriorityIDAsync(_testPriorityID)),
            new Tuple<string, Func<Task>>("GetAllPrioritiesAsync", GetAllPrioritiesAsync),

            // Methods to test Project-related functionality:
            new Tuple<string, Func<Task>>("GetProjectForProjectIDAsync",
                                          () => GetProjectForProjectIDAsync(_testProjectID)),
            new Tuple<string, Func<Task>>("GetAllProjectsAsync", GetAllProjectsAsync),

            // Methods to test ServiceLevel-related functionality:
            new Tuple<string, Func<Task>>("GetServiceLevelForServiceLevelIDAsync",
                                          () => GetServiceLevelForServiceLevelIDAsync(_testServiceLevelID)),
            new Tuple<string, Func<Task>>("GetAllServiceLevelsAsync", GetAllServiceLevelsAsync),


            // Methods to test ServiceLevelAgreement-related functionality:
            new Tuple<string, Func<Task>>("GetServiceLevelAgreementForServiceLevelAgreementIDAsync",
                                          () => GetServiceLevelAgreementForServiceLevelAgreementIDAsync(_testServiceLevelAgreementID)),
            new Tuple<string, Func<Task>>("GetAllServiceLevelAgreementsAsync", GetAllServiceLevelAgreementsAsync),

            // Methods to test ServiceLevelSeverity-related functionality:
            new Tuple<string, Func<Task>>("GetServiceLevelSeverityForSeverityIDAsync",
                                          () => GetServiceLevelSeverityForSeverityIDAsync(_testServiceLevelSeverityID)),
            new Tuple<string, Func<Task>>("GetAllServiceLevelSeveritiesAsync", GetAllServiceLevelSeveritiesAsync),

            // Methods to test ServiceLevelTerm-related functionality:
            new Tuple<string, Func<Task>>("GetServiceLevelTermForServiceLevelTermIDAsync",
                                          () => GetServiceLevelTermForServiceLevelTermIDAsync(_testServiceLevelTermID)),
            new Tuple<string, Func<Task>>("GetAllServiceLevelTermsAsync", GetAllServiceLevelTermsAsync),

            // Methods to test Substatus-related functionality:
            new Tuple<string, Func<Task>>("GetSubstatusForSubstausIDAsync",
                                          () => GetSubstatusForSubstatusIDAsync(_testSubstatusID)),
            new Tuple<string, Func<Task>>("GetAllSubstatusesAsync", GetAllSubstatusesAsync),


            // Methods to test TimeZone-related functionality:
            new Tuple<string, Func<Task>>("GetTimeZoneForTimeZoneIDAsync",
                                          () => GetTimeZoneForTimeZoneIDAsync(_testTimezoneID)),
            new Tuple<string, Func<Task>>("GetAllTimeZonesAsync", GetAllTimeZonesAsync),

            // Methods to test User-related functionality:
            new Tuple<string, Func<Task>>("CreateUserAsync", () => CreateUserAsync(GenerateTestCreateUserDTOInstance())),
            new Tuple<string, Func<Task>>("UpdateUserAsync", () => UpdateUserAsync(GenerateTestUpdateUserDTOInstance())),
            new Tuple<string, Func<Task>>("UpdateUserPasswordAsync", () => UpdateUserPasswordAsync(GenerateTestUpdateUserPasswordDTOInstance())),
            new Tuple<string, Func<Task>>("InactivateUserAsync", () => InactivateUserAsync(GenerateTestInactivateUserDTOInstance())),
            new Tuple<string, Func<Task>>("GetUserForUserIDAsync", () => GetUserForUserIDAsync(_testUserID)),
            new Tuple<string, Func<Task>>("GetAllUsersAsync", GetAllUsersAsync),

            // Methods to test UserDefinedFieldType-related functionality:
            new Tuple<string, Func<Task>>("GetUserDefinedFieldTypeForUserDefinedFieldTypeIDAsync",
                                          () => GetUserDefinedFieldTypeForUserDefinedFieldTypeIDAsync(_testUserdefinedfieldtypeID)),
            new Tuple<string, Func<Task>>("GetAllUserDefinedFieldTypesAsync", GetAllUserDefinedFieldTypesAsync),


            // Methods to test UserType-related functionality:
            new Tuple<string, Func<Task>>("GetUserTypeForUserTypeIDAsync",
                                          () => GetUserTypeForUserTypeIDAsync(_testUserTypeID)),
            new Tuple<string, Func<Task>>("GetAllUserTypesAsync", GetAllUserTypesAsync),
        };

        /// <summary>   The temporary files generated during the execution of the example program. </summary>
        private static readonly List<string> _temporaryFiles = new List<string>();

        #endregion

        #region Private Static Properties

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the base URL for the API from the application configuration. </summary>
        ///
        /// <value> The base API URL. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static string BaseApiurl
        {
            get
            {
                return ConfigurationManager.AppSettings["BaseAPIURL"];
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the current API key from the application configuration. </summary>
        ///
        /// <value> The API key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static string APIKey
        {
            get
            {
                return ConfigurationManager.AppSettings["APIKey"];
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the API version from the application configuration. </summary>
        ///
        /// <value> The API version. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static int APIVersion
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["APIVersion"]);
            }
        }

        #endregion

        #region Private Static Methods

        #region Helper Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Writes the API response to output. </summary>
        ///
        /// <remarks>   09/15/2014. </remarks>
        ///
        /// <typeparam name="TResponse">    Type of the response. </typeparam>
        /// <param name="description">  The description. </param>
        /// <param name="response">     The response. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void WriteAPIResponseToOutput<TResponse>(string description, IssuetrakAPIResponse<TResponse> response)
            where TResponse : class
        {
            Console.WriteLine("/////////////////////////////////////////////////////");

            Console.WriteLine("{0} Response with Status Code: {1} ({2})", description, (int)response.ResponseStatusCode, response.ResponseStatusCode);

            Console.WriteLine("/////////////////////////////////////////////////////");

            // Generate a randomly-named temporary file, and add the ".txt" text file extension so that the file 
            // can be opened with the system-defined text editor.
            var temporaryOutputFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var temporaryFilename = string.Concat(temporaryOutputFilePath, ".txt");

            // Add the temporary file for cleanup at the program's end.
            _temporaryFiles.Add(temporaryFilename);

            // The Formatting.Indented option is used to pretty-print the JSON response text.
            string formattedJson;

            try
            {
                if (response.ResponseObject != null)
                {
                    formattedJson = JsonConvert.SerializeObject(response.ResponseObject, Formatting.Indented);
                }
                else
                {
                    formattedJson = response.ResponseText ?? "Empty Response!";
                }
            }
            catch
            {
                formattedJson = response.ResponseText ?? "Empty Response!";
            }

            File.WriteAllText(temporaryFilename, formattedJson);

            // Display the response output using the current text editor.
            Process.Start(temporaryFilename);
        }

        #endregion        

        #region Control Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Displays a method menu. </summary>
        ///
        /// <remarks>   11/18/2014. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void DisplayMethodMenu()
        {
            Console.WriteLine("Choose a method to execute by entering the numeral for the selected method.");
            Console.WriteLine("The results of the test method executions will be displayed within the system text editor.");

            var methodIndex = 0;

            _testMethodDefinitions.ForEach(tmd => Console.WriteLine("{0,3}:  {1}", ++methodIndex, tmd.Item1));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Process the selected method. </summary>
        ///
        /// <remarks>   11/18/2014. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void ProcessSelectedMethod()
        {
            int selectedMethodIndex;

            while
                (
                    (int.TryParse(Console.ReadLine(), out selectedMethodIndex) == false) || (selectedMethodIndex < 0) || (selectedMethodIndex > _testMethodDefinitions.Count)
                )
            {
                Console.WriteLine("An invalid test method index was entered.  Please try again.");
            }

            var sw = new Stopwatch();

            sw.Start();

            Console.WriteLine("Executing:  {0}...", _testMethodDefinitions[selectedMethodIndex - 1].Item1);

            // Execute the samples within an async delegate that can be started
            // via the static Task.Run method.
            Task.Run(async () =>
            {
                await _testMethodDefinitions[selectedMethodIndex - 1].Item2();
            })
            .Wait();

            sw.Stop();

            Console.WriteLine("Total Method Execution Time (ms):  " + sw.ElapsedMilliseconds);
        }

        #endregion
        
        #region Attachment API Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates attachment asynchronously. </summary>
        ///
        /// <remarks>   09/15/2014. </remarks>
        ///
        /// <param name="createAttachmentDTO">  The create attachment DTO. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task CreateAttachmentAsync(CreateAttachmentDTO createAttachmentDTO)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new CreateAttachmentRequest(BaseApiurl, APIVersion, createAttachmentDTO);

                var response = await client.CreateAttachmentAsync(request);

                WriteAPIResponseToOutput("CreateAttachmentAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets an attachment for attachment identifier asynchronously. </summary>
        ///
        /// <remarks>   09/15/2014. </remarks>
        ///
        /// <param name="attachmentID"> Identifier for the attachment. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAttachmentForAttachmentIDAsync(int attachmentID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAttachmentForAttachmentIDRequest(BaseApiurl, APIVersion, attachmentID);

                var response = await client.GetAttachmentForAttachmentIDAsync(request);

                WriteAPIResponseToOutput("GetAttachmentForAttachmentIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets attachments for issue number asynchronously. </summary>
        ///
        /// <remarks>   09/15/2014. </remarks>
        ///
        /// <param name="issueNumber">  The issue number. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAttachmentsForIssueNumberAsync(int issueNumber)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAttachmentsForIssueNumberRequest(BaseApiurl, APIVersion, issueNumber);

                var response = await client.GetAttachmentsForIssueNumberAsync(request);

                WriteAPIResponseToOutput("GetAttachmentsForIssueNumberAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets compressed attachments for an issue number asynchronously. </summary>
        ///
        /// <remarks>   02/24/2015. </remarks>
        ///
        /// <param name="issueNumber">  The issue number. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAttachmentsInCompressedArchiveForIssueNumberAsync(int issueNumber)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAttachmentsInCompressedArchiveForIssueNumberRequest(BaseApiurl, APIVersion, issueNumber);

                var response = await client.GetAttachmentsInCompressedArchiveForIssueNumberAsync(request);

                WriteAPIResponseToOutput("GetAttachmentsInCompressedArchiveForIssueNumberAsync", response);
            }
        }

        #endregion

        #region Cause API Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a Cause for a Cause ID asynchronously. </summary>
        ///
        /// <remarks>   12/15/2014. </remarks>
        ///
        /// <param name="causeID">  Identifier for the cause. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetCauseForCauseIDAsync(int causeID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetCauseForCauseIDRequest(BaseApiurl, APIVersion, causeID);

                var response = await client.GetCauseForCauseIDAsync(request);

                WriteAPIResponseToOutput("GetCauseForCauseIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all Causes asynchronously. </summary>
        ///
        /// <remarks>   12/15/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllCausesAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllCausesRequest(BaseApiurl, APIVersion);

                var response = await client.GetAllCausesAsync(request);

                WriteAPIResponseToOutput("GetAllCausesAsync", response);
            }
        }

        #endregion

        #region Class API Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a Class for a Class ID asynchronously. </summary>
        ///
        /// <remarks>   12/15/2014. </remarks>
        ///
        /// <param name="classID">  Identifier for the class. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetClassForClassIDAsync(int classID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetClassForClassIDRequest(BaseApiurl, APIVersion, classID);

                var response = await client.GetClassForClassIDAsync(request);

                WriteAPIResponseToOutput("GetClassForClassIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all Classes asynchronously. </summary>
        ///
        /// <remarks>   12/15/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllClassesAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllClassesRequest(BaseApiurl, APIVersion);

                var response = await client.GetAllClassesAsync(request);

                WriteAPIResponseToOutput("GetAllClassesAsync", response);
            }
        }

        #endregion

        #region Department Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets department for a department ID asynchronously. </summary>
        ///
        /// <remarks>   11/25/2014. </remarks>
        ///
        /// <param name="departmentID"> Identifier for the department. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetDepartmentForDepartmentIDAsync(int departmentID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetDepartmentForDepartmentIDRequest(BaseApiurl, APIVersion, departmentID);

                var response = await client.GetDepartmentForDepartmentIDAsync(request);

                WriteAPIResponseToOutput("GetDepartmentForDepartmentIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all departments asynchronously. </summary>
        ///
        /// <remarks>   11/25/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllDepartmentsAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllDepartmentsRequest(BaseApiurl, APIVersion)
                {
                    ShouldIncludeRequestLogging = true
                };

                var response = await client.GetAllDepartmentsAsync(request);

                WriteAPIResponseToOutput("GetAllDepartmentsAsync", response);
            }
        }

        #endregion
        
        #region Location Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets location for a location ID asynchronously. </summary>
        ///
        /// <remarks>   12/05/2014. </remarks>
        ///
        /// <param name="locationID">   Identifier for the location. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetLocationForLocationIDAsync(string locationID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetLocationForLocationIDRequest(BaseApiurl, APIVersion, locationID);

                var response = await client.GetLocationForLocationIDAsync(request);

                WriteAPIResponseToOutput("GetLocationForLocationIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all locations asynchronously. </summary>
        ///
        /// <remarks>   12/05/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllLocationsAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllLocationsRequest(BaseApiurl, APIVersion)
                {
                    ShouldIncludeRequestLogging = true
                };

                var response = await client.GetAllLocationsAsync(request);

                WriteAPIResponseToOutput("GetAllLocationsAsync", response);
            }
        }

        private static async Task CreateLocationsAsync(CreateLocationDTO createLocationModel)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new CreateLocationsRequest(BaseApiurl, APIVersion, createLocationModel)
                {
                    ShouldIncludeRequestLogging = true
                };

                var response = await client.CreateLocationAsync(request);

                WriteAPIResponseToOutput("CreateLocationsAsync", response);
            }
        }

        private static async Task UpdateLocationsAsync(UpdateLocationDTO updateLocationModel)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new UpdateLocationsRequest(BaseApiurl, APIVersion, updateLocationModel)
                {
                    ShouldIncludeRequestLogging = true
                };

                var response = await client.UpdateLocationAsync(request);

                WriteAPIResponseToOutput("UpdateLocationsAsync", response);
            }
        }

        #endregion

        #region Menu Item Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets menu item for a menu item ID asynchronously. </summary>
        ///
        /// <remarks>   03/24/2014. </remarks>
        ///
        /// <param name="menuItemID">   Identifier for the menu item. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetMenuItemForMenuItemIDAsync(int menuItemID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetMenuItemForMenuItemIDRequest(BaseApiurl, APIVersion, menuItemID);

                var response = await client.GetMenuItemForMenuItemIDAsync(request);

                WriteAPIResponseToOutput("GetMenuItemForMenuItemIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all menu items asynchronously. </summary>
        ///
        /// <remarks>   03/24/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllMenuItemsAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllMenuItemsRequest(BaseApiurl, APIVersion)
                {
                    ShouldIncludeRequestLogging = true
                };

                var response = await client.GetAllMenuItemsAsync(request);

                WriteAPIResponseToOutput("GetAllMenuItemsAsync", response);
            }
        }

        #endregion

        #region Note Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Generates a test CreateNoteDTO instance. </summary>
        ///
        /// <remarks>   11/18/2014. </remarks>
        ///
        /// <returns>   The test CreateNoteDTO instance. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static CreateNoteDTO GenerateTestCreateNoteDTOInstance()
        {
            return new CreateNoteDTO
            {
                CreatedBy = "APIUser",
                CreatedDate = DateTime.Now,
                IsPrivate = false,
                IsRichText = false,
                IssueNumber = 122,
                NoteText = "Test Note Text",
                ShouldSuppressEmailForCreateOperation = true
            };
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets note for a note ID asynchronously. </summary>
        ///
        /// <remarks>   09/15/2014. </remarks>
        ///
        /// <param name="noteID">   Identifier for the note. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetNoteForNoteIDAsync(int noteID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetNoteForNoteIDRequest(BaseApiurl, APIVersion, noteID);

                var response = await client.GetNoteForNoteIDAsync(request);

                WriteAPIResponseToOutput("GetNoteForNoteIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets notes for an issue number asynchronously. </summary>
        ///
        /// <remarks>   09/15/2014. </remarks>
        ///
        /// <param name="issueNumber">  The issue number. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetNotesForIssueNumberAsync(int issueNumber)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetNotesForIssueNumberRequest(BaseApiurl, APIVersion, issueNumber)
                {
                    ShouldIncludeRequestLogging = true
                };

                var response = await client.GetNotesForIssueNumberAsync(request);

                WriteAPIResponseToOutput("GetNotesForIssueNumberAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates a note asynchronously. </summary>
        ///
        /// <remarks>   09/15/2014. </remarks>
        ///
        /// <param name="createNoteDTO">    The create note DTO. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task CreateNoteAsync(CreateNoteDTO createNoteDTO)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new CreateNoteRequest(BaseApiurl, APIVersion, createNoteDTO);

                var response = await client.CreateNoteAsync(request);

                WriteAPIResponseToOutput("CreateNoteAsync", response);
            }
        }

        #endregion

        #region Issue API Methods

        private static CreateIssueDTO GenerateTestCreateIssueDTOInstance()
        {
            return new CreateIssueDTO
            {
                ShouldSuppressEmailForCreateOperation = true,
                EnteredBy = _testUserID,
                SubmittedBy = _testUserID,
                SubmittedDate = DateTime.Now,
                Subject = "Test Subject",
                Description = "Issue Description",
                IssueTypeID = 1,
                PriorityID = 1,
                OrganizationID = 1,

            };
        }

        private static UpdateIssueDTO GenerateTestUpdateIssueDTOInstance()
        {
            return new UpdateIssueDTO
            {
                IssueNumber = 2,
                SubmittedBy = _testUserID,
                Subject = "Updated Subject",
                Description = "Updated Description",
                Status = "Open",
                IssueTypeID = 1,
                PriorityID = 1,
                OrganizationID = 1,
            };
        }

        private static SearchIssueDTO GenerateTestSearchIssueDTOInstance()
        {
            // The SearchIssueDTO will consist of one QuerySet with two search filters.
            return new SearchIssueDTO
            {
                // Don't include notes in the Issue result set.
                CanIncludeNotes = false,
                // Start on the first page of results.
                PageIndex = 0,
                // Return ten Issues per page of results.
                PageSize = 10,
                // Order by IssueNumber, ascending.
                QueryOrderingDefinitions = new List<SearchQueryOrderingDTO>
                {
                    new SearchQueryOrderingDTO
                    {
                        FieldName = "IssueNumber",
                        QueryOrderingDirection = SearchQueryOrderingDTO.QueryOrderingDirections.Asc
                    }
                },
                QuerySetDefinitions = new List<SearchQuerySetDTO>
                {
                    new SearchQuerySetDTO
                    {
                        QuerySetIndex = 1,
                        QuerySetOperator = SearchQuerySetDTO.QuerySetOperators.And,
                        QuerySetExpressions = new List<SearchQueryExpressionDTO>
                        {
                            // Exclude the Issue IssueNumber = 122
                            new SearchQueryExpressionDTO
                            {
                                FieldName = "IssueNumber",
                                FieldFilterValue1 = "122",
                                FieldFilterValue2 = null,
                                QueryExpressionOperation = SearchQueryExpressionDTO.QueryExpressionOperations.NotEqual,
                                QueryExpressionOperator = SearchQueryExpressionDTO.QueryExpressionOperators.And
                            },
                            // Include Issues with the term "volatility" in the Description field.
                            new SearchQueryExpressionDTO
                            {
                                FieldName = "Description",
                                FieldFilterValue1 = "volatility",
                                FieldFilterValue2 = null,
                                QueryExpressionOperation = SearchQueryExpressionDTO.QueryExpressionOperations.Contains,
                                QueryExpressionOperator = SearchQueryExpressionDTO.QueryExpressionOperators.And
                            }
                        }
                    }
                }
            };
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets an issue for issue number asynchronously. </summary>
        ///
        /// <remarks>   09/15/2014. </remarks>
        ///
        /// <param name="issueNumber">          The issue number. </param>
        /// <param name="shouldIncludeNotes">   true if should include notes. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetIssueForIssueNumberAsync(int issueNumber, bool shouldIncludeNotes)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetIssueForIssueNumberRequest(BaseApiurl, APIVersion, issueNumber, shouldIncludeNotes);

                var response = await client.GetIssueForIssueNumberAsync(request);

                WriteAPIResponseToOutput("GetIssueForIssueNumberAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates an issue asynchronously. </summary>
        ///
        /// <remarks>   09/15/2014. </remarks>
        ///
        /// <param name="createIssueDTO">   The create issue DTO. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task CreateIssueAsync(CreateIssueDTO createIssueDTO)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new CreateIssueRequest(BaseApiurl, APIVersion, createIssueDTO)
                {
                    ShouldIncludeRequestLogging = true
                };

                var response = await client.CreateIssueAsync(request);

                WriteAPIResponseToOutput("CreateIssueAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates the issue described by updateIssueDTO asynchronously. </summary>
        ///
        /// <remarks>   09/15/2014. </remarks>
        ///
        /// <param name="updateIssueDTO">   The update issue DTO. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task UpdateIssueAsync(UpdateIssueDTO updateIssueDTO)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new UpdateIssueRequest(BaseApiurl, APIVersion, updateIssueDTO)
                {
                    ShouldIncludeRequestLogging = false
                };

                var response = await client.UpdateIssueAsync(request);

                WriteAPIResponseToOutput("UpdateIssueAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets issues for issue number list asynchronously. </summary>
        ///
        /// <remarks>   09/15/2014. </remarks>
        ///
        /// <param name="issueNumbers">         The issue numbers. </param>
        /// <param name="shouldIncludeNotes">   true if should include notes. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetIssuesForIssueNumberListAsync(IEnumerable<int> issueNumbers, bool shouldIncludeNotes)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetIssuesForIssueNumberListRequest(BaseApiurl, APIVersion, issueNumbers, shouldIncludeNotes);

                var response = await client.GetIssuesForIssueNumberListAsync(request);

                WriteAPIResponseToOutput("GetIssuesForIssueNumberListAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Searches for issues asynchronously. </summary>
        ///
        /// <remarks>   11/17/2014. </remarks>
        ///
        /// <param name="searchIssueDTO">   The search issue DTO. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task SearchIssuesAsync(SearchIssueDTO searchIssueDTO)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new SearchIssuesRequest(BaseApiurl, APIVersion, searchIssueDTO)
                {
                    ShouldIncludeRequestLogging = false
                };

                var response = await client.SearchIssuesAsync(request);

                WriteAPIResponseToOutput("SearchIssuesAsync", response);
            }
        }

        #endregion

        #region IssueType Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets issueType for a issueType ID asynchronously. </summary>
        ///
        /// <remarks>   12/02/2014. </remarks>
        ///
        /// <param name="issueTypeID">  Identifier for the issueType. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetIssueTypeForIssueTypeIDAsync(int issueTypeID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetIssueTypeForIssueTypeIDRequest(BaseApiurl, APIVersion, issueTypeID);

                var response = await client.GetIssueTypeForIssueTypeIDAsync(request);

                WriteAPIResponseToOutput("GetIssueTypeForIssueTypeIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all issueTypes asynchronously. </summary>
        ///
        /// <remarks>   12/02/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllIssueTypesAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllIssueTypesRequest(BaseApiurl, APIVersion)
                {
                    ShouldIncludeRequestLogging = true
                };

                var response = await client.GetAllIssueTypesAsync(request);

                WriteAPIResponseToOutput("GetAllIssueTypesAsync", response);
            }
        }

        #endregion

        #region IssueSubType Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets issueSubType for a issueSubType ID asynchronously. </summary>
        ///
        /// <remarks>   12/03/2014. </remarks>
        ///
        /// <param name="issueSubTypeID">   Identifier for the issueSubType. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetIssueSubTypeForIssueSubTypeIDAsync(int issueSubTypeID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetIssueSubTypeForIssueSubTypeIDRequest(BaseApiurl, APIVersion, issueSubTypeID);

                var response = await client.GetIssueSubTypeForIssueSubTypeIDAsync(request);

                WriteAPIResponseToOutput("GetIssueSubTypeForIssueSubTypeIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all issueSubTypes asynchronously. </summary>
        ///
        /// <remarks>   12/03/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllIssueSubTypesAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllIssueSubTypesRequest(BaseApiurl, APIVersion)
                {
                    ShouldIncludeRequestLogging = true
                };

                var response = await client.GetAllIssueSubTypesAsync(request);

                WriteAPIResponseToOutput("GetAllIssueSubTypesAsync", response);
            }
        }

        #endregion

        #region IssueSubType2 Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets issueSubType2 for a issueSubType2 ID asynchronously. </summary>
        ///
        /// <remarks>   12/04/2014. </remarks>
        ///
        /// <param name="issueSubType2ID">  Identifier for the issueSubType2. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetIssueSubType2ForIssueSubType2IDAsync(int issueSubType2ID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetIssueSubType2ForIssueSubType2IDRequest(BaseApiurl, APIVersion, issueSubType2ID);

                var response = await client.GetIssueSubType2ForIssueSubType2IDAsync(request);

                WriteAPIResponseToOutput("GetIssueSubType2ForIssueSubType2IDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all issueSubTypes2 asynchronously. </summary>
        ///
        /// <remarks>   12/04/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllIssueSubTypes2Async()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllIssueSubTypes2Request(BaseApiurl, APIVersion)
                {
                    ShouldIncludeRequestLogging = true
                };

                var response = await client.GetAllIssueSubTypes2Async(request);

                WriteAPIResponseToOutput("GetAllIssueSubTypes2Async", response);
            }
        }

        #endregion

        #region IssueSubType3 Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets issueSubType3 for a issueSubType3 ID asynchronously. </summary>
        ///
        /// <remarks>   12/04/2014. </remarks>
        ///
        /// <param name="issueSubType3ID">  Identifier for the issueSubType3. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetIssueSubType3ForIssueSubType3IDAsync(int issueSubType3ID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetIssueSubType3ForIssueSubType3IDRequest(BaseApiurl, APIVersion, issueSubType3ID);

                var response = await client.GetIssueSubType3ForIssueSubType3IDAsync(request);

                WriteAPIResponseToOutput("GetIssueSubType3ForIssueSubType3IDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all issueSubTypes3 asynchronously. </summary>
        ///
        /// <remarks>   12/04/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllIssueSubTypes3Async()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllIssueSubTypes3Request(BaseApiurl, APIVersion)
                {
                    ShouldIncludeRequestLogging = true
                };

                var response = await client.GetAllIssueSubTypes3Async(request);

                WriteAPIResponseToOutput("GetAllIssueSubTypes3Async", response);
            }
        }

        #endregion

        #region IssueSubType4 Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets issueSubType4 for a issueSubType4 ID asynchronously. </summary>
        ///
        /// <remarks>   12/04/2014. </remarks>
        ///
        /// <param name="issueSubType4ID">  Identifier for the issueSubType4. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetIssueSubType4ForIssueSubType4IDAsync(int issueSubType4ID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetIssueSubType4ForIssueSubType4IDRequest(BaseApiurl, APIVersion, issueSubType4ID);

                var response = await client.GetIssueSubType4ForIssueSubType4IDAsync(request);

                WriteAPIResponseToOutput("GetIssueSubType4ForIssueSubType4IDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all issueSubTypes4 asynchronously. </summary>
        ///
        /// <remarks>   12/04/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllIssueSubTypes4Async()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllIssueSubTypes4Request(BaseApiurl, APIVersion)
                {
                    ShouldIncludeRequestLogging = true
                };

                var response = await client.GetAllIssueSubTypes4Async(request);

                WriteAPIResponseToOutput("GetAllIssueSubTypes4Async", response);
            }
        }

        #endregion

        #region Organization Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets organization for a organization ID asynchronously. </summary>
        ///
        /// <remarks>   12/05/2014. </remarks>
        ///
        /// <param name="organizationID">   Identifier for the organization. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetOrganizationForOrganizationIDAsync(int organizationID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetOrganizationForOrganizationIDRequest(BaseApiurl, APIVersion, organizationID);

                var response = await client.GetOrganizationForOrganizationIDAsync(request);

                WriteAPIResponseToOutput("GetOrganizationForOrganizationIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all organizations asynchronously. </summary>
        ///
        /// <remarks>   12/05/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllOrganizationsAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllOrganizationsRequest(BaseApiurl, APIVersion)
                {
                    ShouldIncludeRequestLogging = true
                };

                var response = await client.GetAllOrganizationsAsync(request);

                WriteAPIResponseToOutput("GetAllOrganizationsAsync", response);
            }
        }

        private static async Task CreateOrganizationAsync(CreateOrganizationDTO createOrganizationDTO)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new CreateOrganizationRequest(BaseApiurl, APIVersion, createOrganizationDTO)
                {
                    ShouldIncludeRequestLogging = true
                };

                var response = await client.CreateOrganizationAsync(request);

                WriteAPIResponseToOutput("CreateOrganizationAsync", response);
            }
        }

        private static async Task UpdateOrganizationAsync(UpdateOrganizationDTO updateOrganizationDTO)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new UpdateOrganizationRequest(BaseApiurl, APIVersion, updateOrganizationDTO)
                {
                    ShouldIncludeRequestLogging = true
                };

                var response = await client.UpdateOrganizationAsync(request);

                WriteAPIResponseToOutput("UpdateOrganizationAsync", response);
            }
        }

        #endregion

        #region Priority API Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a Priority for a Priority ID asynchronously. </summary>
        ///
        /// <remarks>   12/22/2014. </remarks>
        ///
        /// <param name="priorityID">  Identifier for the priority. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetPriorityForPriorityIDAsync(int priorityID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetPriorityForPriorityIDRequest(BaseApiurl, APIVersion, priorityID);

                var response = await client.GetPriorityForPriorityIDAsync(request);

                WriteAPIResponseToOutput("GetPriorityForPriorityIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all Priorities asynchronously. </summary>
        ///
        /// <remarks>   12/22/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllPrioritiesAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllPrioritiesRequest(BaseApiurl, APIVersion);

                var response = await client.GetAllPrioritiesAsync(request);

                WriteAPIResponseToOutput("GetAllPrioritiesAsync", response);
            }
        }

        #endregion

        #region Project API Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a Project for a Project ID asynchronously. </summary>
        ///
        /// <remarks>   12/23/2014. </remarks>
        ///
        /// <param name="projectID">  Identifier for the project. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetProjectForProjectIDAsync(int projectID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetProjectForProjectIDRequest(BaseApiurl, APIVersion, projectID);

                var response = await client.GetProjectForProjectIDAsync(request);

                WriteAPIResponseToOutput("GetProjectForProjectIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all Projects asynchronously. </summary>
        ///
        /// <remarks>   12/23/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllProjectsAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllProjectsRequest(BaseApiurl, APIVersion);

                var response = await client.GetAllProjectsAsync(request);

                WriteAPIResponseToOutput("GetAllProjectsAsync", response);
            }
        }

        #endregion

        #region ServiceLevel API Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a ServiceLevel for a ServiceLevel ID asynchronously. </summary>
        ///
        /// <remarks>   02/09/2015. </remarks>
        ///
        /// <param name="serviceLevelID">  Identifier for the ServiceLevel. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetServiceLevelForServiceLevelIDAsync(int serviceLevelID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetServiceLevelForServiceLevelIDRequest(BaseApiurl, APIVersion, serviceLevelID);

                var response = await client.GetServiceLevelForServiceLevelIDAsync(request);

                WriteAPIResponseToOutput("GetServiceLevelForServiceLevelIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all ServiceLevel instances asynchronously. </summary>
        ///
        /// <remarks>   02/09/2015. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllServiceLevelsAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllServiceLevelsRequest(BaseApiurl, APIVersion);

                var response = await client.GetAllServiceLevelsAsync(request);

                WriteAPIResponseToOutput("GetAllServiceLevelsAsync", response);
            }
        }

        #endregion

        #region ServiceLevelAgreement API Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a ServiceLevelAgreement for a ServiceLevelAgreement ID asynchronously. </summary>
        ///
        /// <remarks>   02/10/2015. </remarks>
        ///
        /// <param name="serviceLevelAgreementID">  Identifier for the ServiceLevelAgreement. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetServiceLevelAgreementForServiceLevelAgreementIDAsync(int serviceLevelAgreementID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetServiceLevelAgreementForServiceLevelAgreementIDRequest(BaseApiurl, APIVersion, serviceLevelAgreementID);

                var response = await client.GetServiceLevelAgreementForServiceLevelAgreementIDAsync(request);

                WriteAPIResponseToOutput("GetServiceLevelAgreementForServiceLevelAgreementIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all ServiceLevelAgreement instances asynchronously. </summary>
        ///
        /// <remarks>   02/10/2015. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllServiceLevelAgreementsAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllServiceLevelAgreementsRequest(BaseApiurl, APIVersion);

                var response = await client.GetAllServiceLevelAgreementsAsync(request);

                WriteAPIResponseToOutput("GetAllServiceLevelAgreementsAsync", response);
            }
        }

        #endregion

        #region ServiceLevelSeverity API Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a ServiceLevelSeverity for a ServiceLevelSeverity ID asynchronously. </summary>
        ///
        /// <remarks>   02/06/2015. </remarks>
        ///
        /// <param name="severityID">  Identifier for the ServiceLevelSeverity. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetServiceLevelSeverityForSeverityIDAsync(int severityID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetServiceLevelSeverityForSeverityIDRequest(BaseApiurl, APIVersion, severityID);

                var response = await client.GetServiceLevelSeverityForSeverityIDAsync(request);

                WriteAPIResponseToOutput("GetServiceLevelSeverityForSeverityIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all ServiceLevelSeverity instances asynchronously. </summary>
        ///
        /// <remarks>   02/06/2015. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllServiceLevelSeveritiesAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllServiceLevelSeveritiesRequest(BaseApiurl, APIVersion);

                var response = await client.GetAllServiceLevelSeveritiesAsync(request);

                WriteAPIResponseToOutput("GetAllServiceLevelSeveritiesAsync", response);
            }
        }

        #endregion

        #region ServiceLevelTerm API Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a ServiceLevelTerm for a ServiceLevelTerm ID asynchronously. </summary>
        ///
        /// <remarks>   02/06/2015. </remarks>
        ///
        /// <param name="serviceLevelTermID">  Identifier for the ServiceLevelTerm. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetServiceLevelTermForServiceLevelTermIDAsync(int serviceLevelTermID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetServiceLevelTermForServiceLevelTermIDRequest(BaseApiurl, APIVersion, serviceLevelTermID);

                var response = await client.GetServiceLevelTermForServiceLevelTermIDAsync(request);

                WriteAPIResponseToOutput("GetServiceLevelTermForServiceLevelTermIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all ServiceLevelTerm instances asynchronously. </summary>
        ///
        /// <remarks>   02/06/2015. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllServiceLevelTermsAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllServiceLevelTermsRequest(BaseApiurl, APIVersion);

                var response = await client.GetAllServiceLevelTermsAsync(request);

                WriteAPIResponseToOutput("GetAllServiceLevelTermsAsync", response);
            }
        }

        #endregion

        #region Substatus API Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a Substatus for a Substatus ID asynchronously. </summary>
        ///
        /// <remarks>   12/15/2014. </remarks>
        ///
        /// <param name="substatusID">  Identifier for the substatus. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetSubstatusForSubstatusIDAsync(int substatusID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetSubstatusForSubstatusIDRequest(BaseApiurl, APIVersion, substatusID);

                var response = await client.GetSubstatusForSubstatusIDAsync(request);

                WriteAPIResponseToOutput("GetSubstatusForSubstatusIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all Substatuses asynchronously. </summary>
        ///
        /// <remarks>   12/15/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllSubstatusesAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllSubstatusesRequest(BaseApiurl, APIVersion);

                var response = await client.GetAllSubstatusesAsync(request);

                WriteAPIResponseToOutput("GetAllSubstatusesAsync", response);
            }
        }

        #endregion

        #region TimeZone API Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a TimeZone for a TimeZone ID asynchronously. </summary>
        ///
        /// <remarks>   03/12/2014. </remarks>
        ///
        /// <param name="timeZoneID">  Identifier for the time zone. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetTimeZoneForTimeZoneIDAsync(int timeZoneID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetTimeZoneForTimeZoneIDRequest(BaseApiurl, APIVersion, timeZoneID);

                var response = await client.GetTimeZoneForTimeZoneIDAsync(request);

                WriteAPIResponseToOutput("GetTimeZoneForTimeZoneIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all TimeZones asynchronously. </summary>
        ///
        /// <remarks>   03/12/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllTimeZonesAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllTimeZonesRequest(BaseApiurl, APIVersion);

                var response = await client.GetAllTimeZonesAsync(request);

                WriteAPIResponseToOutput("GetAllTimeZonesAsync", response);
            }
        }

        #endregion

        #region User Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Generates a test CreateUserDTO instance. </summary>
        ///
        /// <remarks>   03/05/2015 </remarks>
        ///
        /// <returns>   The test CreateUserDTO instance. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static CreateUserDTO GenerateTestCreateUserDTOInstance()
        {
            var testRunDate = DateTime.Now;

            return new CreateUserDTO
            {
                UserID = string.Format("TestUser{0}", testRunDate.ToString("MM-dd-yyyy-hhmmss")),
                UserTypeID = _testUserTypeID,
                Password = "Test.Password.1234",
                FirstName = "Test",
                LastName = "User",
                DisplayName = "Test.User",
                Address1 = "Test Address 1",
                Address2 = "Test Address 2",
                City = "Test City",
                State = "Test State",
                ZipCode = "12345-6789",
                Country = "United States",
                EmailAddress = "test.user@test.com",
                Phone = "757-555-1111",
                Pager = "757-555-2222",
                OrganizationID = 1,
                DepartmentID = null,
                LocationID = null,
                IsActive = true,
                ShouldShowDebug = false,
                IsSysAdmin = false,
                CreatedBy = _testUserID,
                CreatedDate = testRunDate,
                ModifiedBy = _testUserID,
                ModifiedDate = testRunDate,
                LastLoginDate = null,
                CannotLogin = false,
                HasNoAuthentication = false,
                LastPasswordChange = null,
                LoginAttempts = 0,
                UserDefined1ID = null,
                UserDefined1 = null,
                UserDefined2ID = null,
                UserDefined2 = null,
                UserDefined3ID = null,
                UserDefined3 = null,
                UserDefinedDate = null,
                TimeZoneID = _testTimezoneID,
                DoesTimeZoneUseDaylightSavings = true,
                HomePageID = 1,
                DashboardReload = null,
                ShouldDashboardShowTimer = null,
                DashboardDefaultClass = null,
                UserPhotoBytes = null,
                DashboardDefaultMonths = null,
                RedirectTo = _testRedirectURL,
                ListFormat = _testListFormat
            };
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Generates a test UpdateUserDTO instance. </summary>
        ///
        /// <remarks>   03/10/2015. </remarks>
        ///
        /// <returns>   The test UpdateUserDTO instance. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static UpdateUserDTO GenerateTestUpdateUserDTOInstance()
        {
            var testRunDate = DateTime.Now;

            return new UpdateUserDTO
            {
                UserID = _testUpdateUserID,
                UserNumber = _testUpdateUserNumber,
                UserTypeID = _testEndUserTypeID,
                FirstName = "Test.Inactive",
                LastName = "User",
                DisplayName = "Inactive.User-UPDATE",
                Address1 = "Test Address 1",
                Address2 = "Test Address 2",
                City = "Test City",
                State = "Test State",
                ZipCode = "12345-6789",
                Country = "United States",
                EmailAddress = "test.user@test.com",
                Phone = "757-555-1111",
                Pager = "757-555-2222",
                OrganizationID = 1,
                DepartmentID = null,
                LocationID = null,
                IsActive = true,
                ShouldShowDebug = false,
                IsSysAdmin = false,
                CreatedBy = _testUserID,
                CreatedDate = testRunDate,
                ModifiedBy = _testUserID,
                ModifiedDate = testRunDate,
                LastLoginDate = null,
                CannotLogin = true,
                HasNoAuthentication = true,
                LastPasswordChange = null,
                LoginAttempts = null,
                UserDefined1ID = null,
                UserDefined1 = null,
                UserDefined2ID = null,
                UserDefined2 = null,
                UserDefined3ID = null,
                UserDefined3 = null,
                UserDefinedDate = null,
                TimeZoneID = _testTimezoneID,
                DoesTimeZoneUseDaylightSavings = true,
                HomePageID = 1,
                DashboardReload = null,
                ShouldDashboardShowTimer = null,
                DashboardDefaultClass = null,
                UserPhotoBytes = null,
                DashboardDefaultMonths = null,
                RedirectTo = _testRedirectURL,
                ListFormat = _testListFormat
            };
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Generates a test InactivateUserDTO instance. </summary>
        ///
        /// <remarks>   03/11/2015. </remarks>
        ///
        /// <returns>   The test InactivateUserDTO instance. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static InactivateUserDTO GenerateTestInactivateUserDTOInstance()
        {
            return new InactivateUserDTO
            {
                UserID = "InactiveUser"
            };
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Generates a test UpdateUserPasswordDTO instance. </summary>
        ///
        /// <remarks>   03/12/2015. </remarks>
        ///
        /// <returns>   The test UpdateUserPasswordDTO instance. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static UpdateUserPasswordDTO GenerateTestUpdateUserPasswordDTOInstance()
        {
            return new UpdateUserPasswordDTO
            {
                UserID = "Test-End-User-0",
                Password = string.Format("New.Password.{0}", DateTime.Now.ToString("MM-dd-yyyy-hhmmss"))
            };
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a User for a User ID asynchronously. </summary>
        ///
        /// <remarks>   12/15/2014. </remarks>
        ///
        /// <param name="userID">   Identifier for the user. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetUserForUserIDAsync(string userID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetUserForUserIDRequest(BaseApiurl, APIVersion, userID)
                {
                    CanRetrieveUserPhoto = true
                };

                var response = await client.GetUserForUserIDAsync(request);

                WriteAPIResponseToOutput("GetUserForUserIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all Users asynchronously. </summary>
        ///
        /// <remarks>   12/15/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllUsersAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllUsersRequest(BaseApiurl, APIVersion);

                var response = await client.GetAllUsersAsync(request);

                WriteAPIResponseToOutput("GetAllUsersAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates a user asynchronously. </summary>
        ///
        /// <remarks>   03/05/2015. </remarks>
        ///
        /// <param name="createUserDTO">    The CreateUserDTO instance. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task CreateUserAsync(CreateUserDTO createUserDTO)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new CreateUserRequest(BaseApiurl, APIVersion, createUserDTO);

                var response = await client.CreateUserAsync(request);

                WriteAPIResponseToOutput("CreateUserAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates a user asynchronously. </summary>
        ///
        /// <remarks>   03/10/2015. </remarks>
        ///
        /// <param name="updateUserDTO">    The UpdateUserDTO instance. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task UpdateUserAsync(UpdateUserDTO updateUserDTO)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new UpdateUserRequest(BaseApiurl, APIVersion, updateUserDTO);

                var response = await client.UpdateUserAsync(request);

                WriteAPIResponseToOutput("UpdateUserAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates a user password asynchronously. </summary>
        ///
        /// <remarks>   03/12/2015. </remarks>
        ///
        /// <param name="updateUserPasswordDTO">    The UpdateUserPasswordDTO instance. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task UpdateUserPasswordAsync(UpdateUserPasswordDTO updateUserPasswordDTO)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new UpdateUserPasswordRequest(BaseApiurl, APIVersion, updateUserPasswordDTO);
                
                var response = await client.UpdateUserPasswordAsync(request);

                WriteAPIResponseToOutput("UpdateUserPasswordAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Inactivates a user asynchronously. </summary>
        ///
        /// <remarks>   03/10/2015. </remarks>
        ///
        /// <param name="inactivateUserDTO">    The InactivateUserDTO instance. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task InactivateUserAsync(InactivateUserDTO inactivateUserDTO)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new InactivateUserRequest(BaseApiurl, APIVersion, inactivateUserDTO);

                var response = await client.InactivateUserAsync(request);

                WriteAPIResponseToOutput("InactivateUserAsync", response);
            }
        }

        #endregion

        #region UserDefinedFieldType Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a UserDefinedFieldType for a UserDefinedFieldType ID asynchronously. </summary>
        ///
        /// <remarks>   12/24/2014. </remarks>
        ///
        /// <param name="userDefinedFieldTypeID">   Identifier for the UserDefinedFieldType. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetUserDefinedFieldTypeForUserDefinedFieldTypeIDAsync(int userDefinedFieldTypeID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetUserDefinedFieldTypeForUserDefinedFieldTypeIDRequest(BaseApiurl, APIVersion, userDefinedFieldTypeID);

                var response = await client.GetUserDefinedFieldTypeForUserDefinedFieldTypeIDAsync(request);

                WriteAPIResponseToOutput("GetUserDefinedFieldTypeForUserDefinedFieldTypeIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all UserDefinedFieldTypes asynchronously. </summary>
        ///
        /// <remarks>   12/24/2014. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllUserDefinedFieldTypesAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllUserDefinedFieldTypesRequest(BaseApiurl, APIVersion);

                var response = await client.GetAllUserDefinedFieldTypesAsync(request);

                WriteAPIResponseToOutput("GetAllUserDefinedFieldTypesAsync", response);
            }
        }

        #endregion

        #region UserType Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a UserType for a UserType ID asynchronously. </summary>
        ///
        /// <remarks>   08/10/2015. </remarks>
        ///
        /// <param name="timeZoneID">  Identifier for the time zone. </param>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetUserTypeForUserTypeIDAsync(int timeZoneID)
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetUserTypeForUserTypeIDRequest(BaseApiurl, APIVersion, timeZoneID);

                var response = await client.GetUserTypeForUserTypeIDAsync(request);

                WriteAPIResponseToOutput("GetUserTypeForUserTypeIDAsync", response);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all UserTypes asynchronously. </summary>
        ///
        /// <remarks>   08/10/2015. </remarks>
        ///
        /// <returns>   A Task that represents the asynchronous data operation. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static async Task GetAllUserTypesAsync()
        {
            using (var client = new IssuetrakAPIClient(APIKey))
            {
                var request = new GetAllUserTypesRequest(BaseApiurl, APIVersion);

                var response = await client.GetAllUserTypesAsync(request);

                WriteAPIResponseToOutput("GetAllUserTypesAsync", response);
            }
        }

        #endregion

        #endregion

        #region IDisposeable Implementation

        /// <summary>   Flag: Has Dispose already been called? </summary>
        private bool _isDisposed;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Public implementation of Dispose pattern callable by consumers. </summary>
        ///
        /// <remarks>   05/07/2014. </remarks>
        ///
        /// <seealso cref="M:System.IDisposable.Dispose()"/>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Finaliser. </summary>
        ///
        /// <remarks>   06/17/2014. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        ~Program()
        {
            Dispose(false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Protected implementation of Dispose pattern. </summary>
        ///
        /// <remarks>   05/07/2014. </remarks>
        ///
        /// <param name="isDisposing">
        ///     true to release both managed and unmanaged resources; false to release only unmanaged
        ///     resources.
        /// </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                // Delete any temporary files created.
                _temporaryFiles.ForEach(tempFilePath =>
                {
                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }
                });
            }

            // Free any unmanaged objects here. 
            _isDisposed = true;
        }

        #endregion

        #region Program Public Methods

        //Microsoft implementation for configuration 
        static void ReadAllSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    Console.WriteLine("AppSettings is empty.");
                }
                else
                {
                    foreach (var key in appSettings.AllKeys)
                    {
                        Console.WriteLine("Key: {0} Value: {1}", key, appSettings[key]);
                    }
                }
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
        }

        static void ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "Not Found";
                Console.WriteLine(result);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
        }

        static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes this instance. </summary>
        ///
        /// <remarks>   11/18/2014. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public async virtual void Execute()
        {
            var runIteration = true;

            // The testing process consists of the display of the available test methods, the execution of a selected method,
            // and the option to execute another test.
            while (runIteration)
            {
                Console.Clear();

                DisplayMethodMenu();

                ProcessSelectedMethod();

                Console.WriteLine("Execute another test method?  (Y/N)");

                runIteration = string.Compare(Console.ReadLine() ?? string.Empty, "y", StringComparison.OrdinalIgnoreCase) == 0;
            }
        }


#endregion

////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>   Main entry-point for this application. </summary>
///
/// <remarks>   09/15/2014. </remarks>
////////////////////////////////////////////////////////////////////////////////////////////////////
private static void Main()
        {
            using (var programCore = new UFII())
            {
                programCore.Execute();
            }
        }
    }
}

