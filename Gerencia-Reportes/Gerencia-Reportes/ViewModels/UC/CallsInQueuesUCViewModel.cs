

using DocumentFormat.OpenXml.Spreadsheet;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Gerencia_Reportes.Helpers;
using Gerencia_Reportes.Interfaces;
using Gerencia_Reportes.Models;
using Gerencia_Reportes.Models.Sqlite;
using Gerencia_Reportes.Services;
using Gerencia_Reportes.Utils;
using Microsoft.Win32;
using Notifications.Wpf;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Gerencia_Reportes.ViewModels.UC
{
    public class CallsInQueuesUCViewModel : ViewModelBase
    {
        private IEpicorProvider service;
        private MessagesProvider localStorate;

        private List<CallsInQueues> list;

        private ObservableCollection<CallsInQueues> itemList;

        public ObservableCollection<CallsInQueues> ItemList
        {
            get { return itemList; }
            set
            {
                itemList = value;
                RaisePropertyChanged(nameof(ItemList));
            }
        }


        private CallsInQueues selectedItem;

        public CallsInQueues SelectedItem
        {
            get { return selectedItem; }
            set { selectedItem = value; RaisePropertyChanged(nameof(SelectedItem)); }
        }






        private ObservableCollection<Queue> queuesFilter;

        public ObservableCollection<Queue> QueuesFilter
        {
            get { return queuesFilter; }
            set
            {
                queuesFilter = value;
                RaisePropertyChanged(nameof(QueuesFilter));
            }
        }


        private bool isLoading;

        public bool IsLoading
        {
            get { return isLoading; }

            set
            {
                isLoading = value;
                RaisePropertyChanged(nameof(IsLoading));
            }
        }

        private bool isDialogOpen;

        public bool IsDialogOpen
        {
            get { return isDialogOpen; ;}

            set
            {
                isDialogOpen = value;
                RaisePropertyChanged(nameof(IsDialogOpen));
            }
        }

      




        private int totalRecords;

        public int TotalRecords
        {
            get { return totalRecords; }
            set
            {
                totalRecords = value;
                RaisePropertyChanged(nameof(TotalRecords));
            }
        }


        private string searchByNumber;

        public string SearchByNumber
        {
            get { return searchByNumber; }

            set
            {
                searchByNumber = value;
                RaisePropertyChanged(nameof(SearchByNumber));
            }
        }


        private string valueShortText;

        public string ValueShortText
        {
            get { return valueShortText; }

            set
            {
                valueShortText = value;
                RaisePropertyChanged(nameof(ValueShortText));
            }
        }


        private string searchByAttribute;

        public string SearchByAttribute
        {
            get { return searchByAttribute; }

            set
            {
                searchByAttribute = value;
                RaisePropertyChanged(nameof(SearchByAttribute));
            }
        }


        private string comments;

        public string Comments
        {
            get { return comments; }

            set
            {
                comments = value;
                RaisePropertyChanged(nameof(Comments));
            }
        }


        private Visibility isEditMode;

        public Visibility IsEditMode
        {
            get { return isEditMode; }

            set
            {
                isEditMode = value;
                RaisePropertyChanged(nameof(IsEditMode));
            }
        }

        private int editWidth;

        public int EditWidth
        {
            get { return editWidth; }

            set
            {
                editWidth = value;
                RaisePropertyChanged(nameof(EditWidth));
            }
        }


        private Visibility isNotEditMode;

        public Visibility IsNotEditMode
        {
            get { return isNotEditMode; }

            set
            {
                isNotEditMode = value;
                RaisePropertyChanged(nameof(IsNotEditMode));
            }
        }

        private int isNotWidth;

        public int IsNotWidth
        {
            get { return isNotWidth;  }

            set
            {
                isNotWidth= value;
                RaisePropertyChanged(nameof(IsNotWidth));
            }
        }




        public ICommand SearchCommand { get; private set; }
        public RelayCommand<KeyEventArgs> SearchByNumberKeyDownCommand { get; private set; }
        public RelayCommand<KeyEventArgs> SearchByValueKeyDownCommand { get; private set; }

        public RelayCommand<KeyEventArgs> SearchByAttributeKeyDownCommand { get; private set; }

        public ICommand SearchByCombinationCommand { get; private set; }
        public ICommand ExcelReportCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand OpenDialogCommand { get; private set; }
        public ICommand SendCommentCommand { get; private set; }

        public ICommand DeleteCommentCommand { get; private set; }
        public ICommand UpdateCommentCommand { get; private set; }


        public CallsInQueuesUCViewModel()
        {

            IsLoading = false;
            TotalRecords = 0;
            service = new EpicorProvider();
            localStorate = new MessagesProvider();
            list = new List<CallsInQueues>();

            SearchCommand = new AsyncRelayCommand(SearchAsync);
            SearchByNumberKeyDownCommand = new RelayCommand<KeyEventArgs>(SearchByNumberKeyDown);
            SearchByValueKeyDownCommand = new RelayCommand<KeyEventArgs>(SearchByValueKeyDown);
            SearchByAttributeKeyDownCommand = new RelayCommand<KeyEventArgs>(SearchByAttriabuteKeyDown);
            SearchByCombinationCommand = new AsyncRelayCommand(SearchByCombinationAsync);
            ExcelReportCommand = new RelayCommand(ExcelReport);
            RefreshCommand = new AsyncRelayCommand(RefrehAsync);

            OpenDialogCommand = new RelayCommand<CallsInQueues>(OpenDialog);
            SendCommentCommand = new RelayCommand(SendComment);
            DeleteCommentCommand = new RelayCommand(DeleteComment);
            UpdateCommentCommand = new RelayCommand(UpdateComment);

            Task.Run(async () =>
            {
                await LoadFilters();
                await LoadDataAsync();

            });

        }




        private async Task LoadFilters()
        {
            try
            {
                IsLoading = true;

                var filters = await service.FetchQueuesAsync();

                if (filters != null)
                {
                    QueuesFilter = new ObservableCollection<Queue>(filters);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                NotifiactionMessage
                    .SetMessage("Error", GlobalMessages.INTERNAL_SERVER_ERROR, NotificationType.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadDataAsync(string queryParams = "")
        {
            try
            {
                IsLoading = true;

                //Thread.Sleep(5000);

                list = await service.FetchAllAsync(queryParams);


                if (list != null)
                {
                    var listSqlite = localStorate.GetAll();

                     var result = from SqlServer in list
                                 join Sqlite in listSqlite
                                 on new { SqlServer.Number, SqlServer.Value, SqlServer.Attribute }
                                 equals new { Sqlite.Number, Sqlite.Value, Sqlite.Attribute }
                                 into joined
                                 from Sqlite in joined.DefaultIfEmpty()
                                 select new CallsInQueues
                                 {
                                     Number = SqlServer.Number,
                                     Types = SqlServer.Types,
                                     Summary = SqlServer.Summary,
                                     Queue = SqlServer.Queue,
                                     Status = SqlServer.Status,
                                     Priority = SqlServer.Priority,
                                     OpenDate = SqlServer.OpenDate,
                                     DueDate = SqlServer.DueDate,
                                     Product = SqlServer.Product,
                                     StartDate = SqlServer.StartDate,
                                     DateAssignTo = SqlServer.DateAssignTo,

                                     Attribute = SqlServer.Attribute,
                                     Value = SqlServer.Value,
                                     EventSummary = SqlServer.EventSummary,
                                     Detail = SqlServer.Detail,
                                     Comments = Sqlite != null ? Sqlite.Comments : null
                                 };

                    TotalRecords = list.Count;
                    ItemList = new ObservableCollection<CallsInQueues>(result);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                NotifiactionMessage
                    .SetMessage("Error", GlobalMessages.INTERNAL_SERVER_ERROR, NotificationType.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task SearchAsync()
        {

            try
            {
                var selectedQueues = QueuesFilter.Where(q => q.IsSelected).Select(q => q.Name).ToArray();

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < selectedQueues.Length; i++)
                {
                    sb.Append($"'{selectedQueues[i]}'");

                    if (i < selectedQueues.Length - 1)
                    {
                        sb.Append(", ");
                    }
                }

                if (string.IsNullOrEmpty(sb.ToString()) ||
                    string.IsNullOrWhiteSpace(sb.ToString()))
                {
                    NotifiactionMessage
                    .SetMessage("No Valido", "Es necesario ingresar un valor de busqueda",
                               NotificationType.Error);
                    return;
                }

                string queryParam = $" AND  Que.Name IN ({sb})";

                await LoadDataAsync(queryParam);
                ClearFilters();
                NotifiactionMessage
               .SetMessage("Información", GlobalMessages.SUCCESS,
                       NotificationType.Success);

            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message.ToString());
                NotifiactionMessage
                    .SetMessage("Error", GlobalMessages.INTERNAL_SERVER_ERROR, NotificationType.Error);
            }

        }

        private async void SearchByNumberKeyDown(KeyEventArgs args)
        {
            try
            {
                if (args.Key == Key.Enter)
                {

                    if (string.IsNullOrEmpty(SearchByNumber) ||
                     string.IsNullOrWhiteSpace(SearchByNumber))
                    {
                        NotifiactionMessage
                        .SetMessage("No Valido", "Es necesario ingresar un valor de busqueda",
                                   NotificationType.Error);
                        return;
                    }

                    string queryParam = $" AND Sc.Number LIKE '%{SearchByNumber}%' ";
                    await LoadDataAsync(queryParam);
                    NotifiactionMessage
                   .SetMessage("Información", GlobalMessages.SUCCESS,
                           NotificationType.Success);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                NotifiactionMessage
                    .SetMessage("Error", GlobalMessages.INTERNAL_SERVER_ERROR, NotificationType.Error);
            }
        }

        private async void SearchByValueKeyDown(KeyEventArgs args)
        {
            try
            {
                if (args.Key == Key.Enter)
                {

                    if (string.IsNullOrEmpty(ValueShortText) ||
                     string.IsNullOrWhiteSpace(ValueShortText))
                    {
                        NotifiactionMessage
                        .SetMessage("No Valido", "Es necesario ingresar un valor de busqueda",
                                   NotificationType.Error);
                        return;
                    }
                    string queryParam = $" AND Av.ValueShortText LIKE '%{ValueShortText}%' ";
                    await LoadDataAsync(queryParam);
                    NotifiactionMessage
                   .SetMessage("Información", GlobalMessages.SUCCESS,
                           NotificationType.Success);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                NotifiactionMessage
                    .SetMessage("Error", GlobalMessages.INTERNAL_SERVER_ERROR, NotificationType.Error);
            }
        }


        private async void SearchByAttriabuteKeyDown(KeyEventArgs args)
        {
            try
            {
                if (args.Key == Key.Enter)
                {

                    if (string.IsNullOrEmpty(SearchByAttribute) ||
                     string.IsNullOrWhiteSpace(SearchByAttribute))
                    {
                        NotifiactionMessage
                        .SetMessage("No Valido", "Es necesario ingresar un valor de busqueda",
                                   NotificationType.Error);
                        return;
                    }
                    string queryParam = $" AND Ac.LabelText  LIKE '%{SearchByAttribute}%' ";
                    await LoadDataAsync(queryParam);
                    NotifiactionMessage
                   .SetMessage("Información", GlobalMessages.SUCCESS,
                           NotificationType.Success);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                NotifiactionMessage
                    .SetMessage("Error", GlobalMessages.INTERNAL_SERVER_ERROR, NotificationType.Error);
            }
        }


        public async Task SearchByCombinationAsync()
        {
            string query = string.Empty;
            try
            {
                IsLoading = false;

                //Search By Queue 
                var selectedQueues = QueuesFilter.Where(q => q.IsSelected).Select(q => q.Name).ToArray();

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < selectedQueues.Length; i++)
                {
                    sb.Append($"'{selectedQueues[i]}'");

                    if (i < selectedQueues.Length - 1)
                    {
                        sb.Append(", ");
                    }
                }


                if (string.IsNullOrEmpty(sb.ToString())
                && (string.IsNullOrEmpty(SearchByNumber) || string.IsNullOrWhiteSpace(SearchByNumber))
                && (string.IsNullOrEmpty(ValueShortText) || string.IsNullOrWhiteSpace(ValueShortText))
                && (string.IsNullOrEmpty(SearchByAttribute) || string.IsNullOrWhiteSpace(SearchByAttribute)))
                {
                    NotifiactionMessage
                    .SetMessage("No Valido", "Es necesario ingresar un valor de busqueda",
                               NotificationType.Error);
                    return;
                }


                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    query = $"   AND  Que.Name IN ({sb})";
                }

                //Search By Queue Number

                if (!string.IsNullOrEmpty(SearchByNumber))
                {
                    query += $"   AND Sc.Number LIKE '%{SearchByNumber}%'  ";
                }

                //Search By Value
                if (!string.IsNullOrEmpty(ValueShortText))
                {
                    query += $"  AND Av.ValueShortText LIKE '%{ValueShortText}%' ";
                }


                if (!string.IsNullOrEmpty(SearchByAttribute))
                {
                    query += $"  AND Ac.LabelText  LIKE '%{SearchByAttribute}%'";
                }

                await LoadDataAsync(query);

                NotifiactionMessage
                   .SetMessage("Información", GlobalMessages.REFRESH,
                       NotificationType.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                NotifiactionMessage
                    .SetMessage("Error", GlobalMessages.INTERNAL_SERVER_ERROR, NotificationType.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }


        public async Task RefrehAsync()
        {
            try
            {
                IsLoading = false;
                await LoadFilters();
                await LoadDataAsync(string.Empty);
                ClearFilters();
                NotifiactionMessage
               .SetMessage("Información", GlobalMessages.REFRESH,
                       NotificationType.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                NotifiactionMessage
                    .SetMessage("Error", GlobalMessages.INTERNAL_SERVER_ERROR, NotificationType.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }





        public void ExcelReport()
        {

            try
            {

                DateTime fecha = new DateTime(2024, 1, 15);
                string fechaFormateada = DateTime.Now.ToString("MMMM d yyyy", new CultureInfo("es-ES"));
                fechaFormateada = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fechaFormateada);


                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Guardar archivo Excel";
                saveFileDialog.FileName = $"Reporte SupportCall In Queues- {fechaFormateada}";

                bool? result = saveFileDialog.ShowDialog(); 

                if (result == true)
                {
                    IsLoading = true;

                    string filePath = saveFileDialog.FileName;

                    // Create a new SLDocument
                    SLDocument sl = new SLDocument();


                    sl.SelectWorksheet("Sheet1");
                    sl.RenameWorksheet("Sheet1", "Reporte SupportCall In Queues");


                    sl.SetCellValue(1, 1, "Number");
                    sl.SetCellValue(1, 2, "Types");
                    sl.SetCellValue(1, 3, "Summary");
                    sl.SetCellValue(1, 4, "Queue");
                    sl.SetCellValue(1, 5, "Status");
                    sl.SetCellValue(1, 6, "Priority");
                    sl.SetCellValue(1, 7, "Open Date");
                    sl.SetCellValue(1, 8, "Due Date");
                    sl.SetCellValue(1, 9, "Product");
                    sl.SetCellValue(1, 10, "Start Date");
                    sl.SetCellValue(1, 11, "Date Assign To");
                    sl.SetCellValue(1, 12, "Priority");
                    sl.SetCellValue(1, 13, "Attribute");
                    sl.SetCellValue(1, 14, "Value");
                    sl.SetCellValue(1, 15, "Event Summary");


                    SLStyle styleHeader = sl.CreateStyle();
                    styleHeader.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.DarkBlue, System.Drawing.Color.Empty);
                    styleHeader.Font.Bold = true;
                    styleHeader.Font.FontColor = System.Drawing.Color.White;
                    sl.SetRowStyle(1, styleHeader);


                    for (int i = 0; i < list.Count; i++)
                    {
                        sl.SetCellValue(i + 2, 1, list[i].Number);
                        sl.SetCellValue(i + 2, 2, list[i].Types);
                        sl.SetCellValue(i + 2, 3, list[i].Summary);
                        sl.SetCellValue(i + 2, 4, list[i].Queue);
                        sl.SetCellValue(i + 2, 5, list[i].Status);
                        sl.SetCellValue(i + 2, 6, list[i].Priority);
                        sl.SetCellValue(i + 2, 7, list[i].OpenDate);
                        sl.SetCellValue(i + 2, 8, list[i].DueDate);
                        sl.SetCellValue(i + 2, 9, list[i].Product);
                        sl.SetCellValue(i + 2, 10, list[i].StartDate);
                        sl.SetCellValue(i + 2, 11, list[i].DateAssignTo);
                        sl.SetCellValue(i + 2, 12, list[i].Priority);
                        sl.SetCellValue(i + 2, 13, list[i].Attribute);
                        sl.SetCellValue(i + 2, 14, list[i].Value);
                        sl.SetCellValue(i + 2, 15, list[i].EventSummary);
                    }

                    sl.SaveAs(filePath);
                    NotifiactionMessage
                  .SetMessage("Información", "Reporte de excel ha sido generado con éxito.",
                          NotificationType.Success);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                NotifiactionMessage
                    .SetMessage("Error", GlobalMessages.INTERNAL_SERVER_ERROR, NotificationType.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }


        private void ClearFilters()
        {
            SearchByNumber = string.Empty;
            ValueShortText = string.Empty;
            SearchByAttribute = string.Empty;
        }


        private string Attribute = string.Empty;
        private string Value = string.Empty;
        private int Number = 0;
        private void OpenDialog(CallsInQueues queue)
        {
            Attribute = string.Empty;
            Value = string.Empty;
            Number = 0;
            Comments= string.Empty;
           

            var row = queue;


            if (row != null) 
            {
                Attribute = row.Attribute;
                Value = row.Value;
                Number = row.Number;
                Comments = row.Comments;
                IsDialogOpen = true;

                if (string.IsNullOrEmpty(Comments))
                {
                    IsEditMode = Visibility.Hidden;
                    EditWidth = 0;

                    IsNotEditMode=Visibility.Visible;
                    IsNotWidth= 125;
                }
                else 
                {
                    IsEditMode = Visibility.Visible;
                    EditWidth = 125;

                    IsNotEditMode = Visibility.Hidden;
                    IsNotWidth = 0;
                }

            }
            
        }

        private void SendComment() 
        {


            if (string.IsNullOrEmpty(Comments) ||
                    string.IsNullOrWhiteSpace(Comments))
            {
                NotifiactionMessage
                .SetMessage("No Valido", "Es necesario ingresar un  comentario",
                           NotificationType.Error);
                return;
            }

            try
            {
                var _messages = new Messages
                {
                    Attribute= Attribute,
                    Value= Value,
                    Number= Number,
                    Comments= Comments.Trim(),
                };

                var passValue = SelectedItem;
                int index = ItemList.IndexOf(SelectedItem);

               

                
                 ItemList.Remove(SelectedItem);

                var newValue = passValue;
                newValue.Comments = Comments;

                ItemList.Insert(index, newValue);

                // ItemList.Add(newValue);

                localStorate.Insert(_messages);
                IsDialogOpen = false;

                NotifiactionMessage
                 .SetMessage("Información", "El comentario  ha sido generado con éxito.",
                         NotificationType.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                NotifiactionMessage
                    .SetMessage("Error", GlobalMessages.INTERNAL_SERVER_ERROR, NotificationType.Error);
            }
        }


        private void DeleteComment()
        {
            try
            {
                var _delete = new Messages
                {
                    Attribute = Attribute,
                    Value = Value,
                    Number = Number,
                };

                var passValue = SelectedItem;
                int index = ItemList.IndexOf(SelectedItem);

                ItemList.Remove(SelectedItem);
                var newValue = passValue;
                newValue.Comments = string.Empty;
                ItemList.Insert(index, newValue);
                localStorate.Delete(_delete);
                IsDialogOpen = false;

                NotifiactionMessage
                 .SetMessage("Información", "El comentario  ha sido eliminado con éxito.",
                         NotificationType.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                NotifiactionMessage
                    .SetMessage("Error", GlobalMessages.INTERNAL_SERVER_ERROR, NotificationType.Error);
            }
        }

        private void UpdateComment()
        {


            if (string.IsNullOrEmpty(Comments) ||
                    string.IsNullOrWhiteSpace(Comments))
            {
                NotifiactionMessage
                .SetMessage("No Valido", "Es necesario ingresar un  comentario",
                           NotificationType.Error);
                return;
            }

            try
            {
                var _messages = new Messages
                {
                    Attribute = Attribute,
                    Value = Value,
                    Number = Number,
                    Comments = Comments.Trim(),
                };

                var passValue = SelectedItem;
                int index = ItemList.IndexOf(SelectedItem);

                ItemList.Remove(SelectedItem);
                var newValue = passValue;
                newValue.Comments = Comments;

                ItemList.Insert(index, newValue);

                localStorate.Update(_messages);
                IsDialogOpen = false;

                NotifiactionMessage
                 .SetMessage("Información", "El comentario  ha sido actualizado con éxito.",
                         NotificationType.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                NotifiactionMessage
                    .SetMessage("Error", GlobalMessages.INTERNAL_SERVER_ERROR, NotificationType.Error);
            }
        }

    }
}
