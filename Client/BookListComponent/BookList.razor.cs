using Blazorise;
using Blazorise.DataGrid;
using BookCatalog.Client.Interfaces;
using BookCatalog.Shared.Contracts.Response;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace BookCatalog.Client.BookListComponent
{
    public partial class BookList : ComponentBase, IDisposable
    {
        [Inject] public IBookService? BooksData { get; set; }
        [Inject] public HubConnection? HubConnection { get; set; }
        [Inject] public NavigationManager? NavigationManager { get; set; }
        [Inject] public IJSRuntime? JSRuntime { get; set; }
        [Inject] public ILogger<BookList>? Logger { get; set; }

        private bool modalVisible;
        private bool deleteModalVisible;

        private List<BookResponse>? booksList;
        private BookResponse? selectedBook;
        private int totalItemCount;
        private int pageNumber;
        private int pageSize;
        private Dictionary<string, string> sortParam;

        private IBrowserFile? selectedFile;

        private bool adding;

        private string inputText = string.Empty;

        private DataGrid<BookResponse>? dataGrid;

        private bool disposed = false;

        private async Task OnSearchTextChanged(string newText)
        {
            inputText = newText;

            // Trigger DataGrid to reload its data
            if (dataGrid is not null)
            {
                Logger!.LogDebug("Reloading DataGrid due to search text change.");

                await dataGrid.Reload();
            }
        }

        private async Task TriggerFileUpload()
        {
            Logger!.LogDebug("Triggering file upload dialog.");
            // Use JavaScript to click the hidden file input
            await JSRuntime!.InvokeVoidAsync("triggerFileInputClick", "fileInput");
        }

        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            selectedFile = e.File;

            if (selectedFile is not null)
            {
                Logger!.LogInformation("File selected: {FileName}, Size: {FileSize} bytes", selectedFile.Name, selectedFile.Size);
                await UploadFileToBackend();
            }
            else
            {
                Logger!.LogWarning("No file was selected.");
            }
        }

        private async Task UploadFileToBackend()
        {
            if (selectedFile is null)
            {
                Logger!.LogWarning("No file selected for upload.");
                return;
            }

            try
            {
                Logger!.LogDebug("Uploading file {FileName} to backend.", selectedFile.Name);

                // Create a MultipartFormDataContent for the file
                var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(selectedFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024)); // 10 MB max size
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(selectedFile.ContentType);
                // Set the Content-Disposition header to indicate that this is a file
                streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"File\"",  // Ensure that the name matches the model property in the backend
                    FileName = $"\"{selectedFile.Name}\""  // Set the filename
                };

                content.Add(streamContent, "File", selectedFile.Name);
                content.Add(new StringContent("test value"), "testKey");

                // Send the file to the backend
                var response = await BooksData.AddBooksAsync(content);

                if (response.IsSuccessStatusCode)
                {
                    Logger!.LogInformation("File uploaded successfully: {FileName}", selectedFile.Name);
                }
                else
                {
                    Logger!.LogError("Failed to upload file. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Logger!.LogError(ex, "Error occurred while uploading the file.");
            }
        }


        private Task ShowModal(bool isAdding)
        {
            adding = isAdding;
            if (isAdding)
            {
                selectedBook = new BookResponse();
                Logger!.LogDebug("Preparing to add a new book.");
            }
            
            modalVisible = true;
            return Task.CompletedTask;
        }

        private Task ShowDeleteModal()
        {
            if (selectedBook is null)
            {
                selectedBook = new BookResponse();
                Logger!.LogWarning("Delete operation triggered without a selected book.");
            }
            deleteModalVisible = true;
            Logger!.LogDebug("Delete dialog displayed for book: {Book}", selectedBook.BookKey);
            return Task.CompletedTask;
        }

        private Task SaveBookDetails()
        {
            Logger!.LogInformation("Saving book details: {Book}", selectedBook.BookKey);

            return Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            // Establish the SignalR connection
            HubConnection!.On<string>("ReceiveBookUpdate", async (message) =>
            {
                // Update the DataGrid by refreshing the data
                Logger!.LogInformation("SignalR update received: {Message}", message);
                await RefreshData();
            });

            await HubConnection!.StartAsync();
            Logger!.LogInformation("SignalR connection started.");
        }

        // Refresh the data when an update is received
        private async Task RefreshData()
        {
            Logger!.LogDebug("Refreshing data...");
            
            // Re-fetch the data, you can optionally pass the current page and size if needed
            await GetResponce(pageNumber, pageSize);

            // Trigger a UI update
            StateHasChanged();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this); // Prevents finalization if already disposed
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Dispose managed resources
                if (HubConnection?.State == HubConnectionState.Connected)
                {
                    HubConnection.StopAsync().GetAwaiter().GetResult();
                    
                    Logger!.LogInformation("SignalR connection stopped.");
                }
            }
            
            disposed = true;
        }

        private async Task LoadData(DataGridReadDataEventArgs<BookResponse> args)
        {
            // Prepare the request parameters
            pageNumber = args.Page;
            pageSize = args.PageSize;

            Logger!.LogDebug("Loading data for page {PageNumber} with size {PageSize}.", pageNumber, pageSize);
            
            // Fetch data from server
            await GetResponce(pageNumber, pageSize, sortParam, inputText);
        }

        private void OnSortChanged(DataGridSortChangedEventArgs args)
        {
            // Extract the current sorting info
            var sortField = args.ColumnFieldName;
            var sortOrder = args.SortDirection == SortDirection.Descending ? "desc" : "asc";

            Logger!.LogInformation("Sort changed: Field = {Field}, Order = {Order}", sortField, sortOrder);
            
            
            sortParam = new Dictionary<string, string>
            {
                ["sortField"] = sortField,
                ["sortOrder"] = sortOrder
            };
        }

        private async Task GetResponce(int pageNumber, int pageSize, Dictionary<string, string> sortParam = null, string searchFactor = null)
        {
            Logger!.LogDebug("Fetching data: Page = {Page}, Size = {Size}, Sort = {Sort}, Search = {Search}",
                            pageNumber, pageSize, sortParam, searchFactor);

            var response = await BooksData!.GetBooksAsync(pageNumber, pageSize, sortParam, searchFactor);

            // Update DataGrid
            booksList = response.Data.ToList();
            totalItemCount = response.TotalCount;

            Logger!.LogInformation("Data fetched: {Count} items, Total = {Total}", booksList.Count, totalItemCount);
        }
    }
}
