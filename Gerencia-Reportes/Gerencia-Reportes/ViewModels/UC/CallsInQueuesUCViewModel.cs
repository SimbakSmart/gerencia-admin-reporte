

using DocumentFormat.OpenXml.Spreadsheet;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Gerencia_Reportes.Helpers;
using Gerencia_Reportes.Interfaces;
using Gerencia_Reportes.Models;
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
using System.Windows.Input;

namespace Gerencia_Reportes.ViewModels.UC
{
    public class CallsInQueuesUCViewModel : ViewModelBase
    {
        private IEpicorProvider service;
       // private MessagesRepository localStorate;

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


        public ICommand SearchCommand { get; private set; }
        public RelayCommand<KeyEventArgs> SearchByNumberKeyDownCommand { get; private set; }
        public RelayCommand<KeyEventArgs> SearchByValueKeyDownCommand { get; private set; }

        public RelayCommand<KeyEventArgs> SearchByAttributeKeyDownCommand { get; private set; }

        public ICommand ExcelReportCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public ICommand SendMessageCommand { get; private set; }

        public ICommand OpenDynamicDialogCommand { get; }

        public CallsInQueuesUCViewModel()
        {
            IsLoading = false;
            TotalRecords = 0;
            service = new EpicorProvider();
            //localStorate = new MessagesRepository();
            list = new List<CallsInQueues>();

            SearchCommand = new AsyncRelayCommand(SearchAsync);
            SearchByNumberKeyDownCommand = new RelayCommand<KeyEventArgs>(SearchByNumberKeyDown);
            SearchByValueKeyDownCommand = new RelayCommand<KeyEventArgs>(SearchByValueKeyDown);
            SearchByAttributeKeyDownCommand = new RelayCommand<KeyEventArgs>(SearchByAttriabuteKeyDown);
            ExcelReportCommand = new RelayCommand(ExcelReport);
            RefreshCommand = new AsyncRelayCommand(RefrehAsync);
            SendMessageCommand = new RelayCommand<CallsInQueues>(SendMessageAsync);

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

                    TotalRecords = list.Count;
                    ItemList = new ObservableCollection<CallsInQueues>(list);
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


        public async Task RefrehAsync()
        {
            try
            {
                IsLoading = false;
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

                bool? result = saveFileDialog.ShowDialog(); // Mostrar el cuadro de diálogo

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


        private async void SendMessageAsync(CallsInQueues queue)
        {

            try
            {
                //var queryIdPara = $"  ParentID='{id}' ";
                //var list = await service.FetchAttributesAsync(queryIdPara);

                //var dialog = new AttributeDialog(list);

                //await MaterialDesignThemes.Wpf.DialogHost.Show(dialog);
                //NotifiactionHelper
                //   .SetMessage("Información", "La búsqueda se ha realizado con éxito.",
                //           NotificationType.Success);

                // var dialogContent = new StackPanel();
                //var dialogContent = new StackPanel();

                //var dialogTextBlock = new TextBlock
                //{
                //    Text = dialogViewModel.DialogText,
                //    Margin = new Thickness(20)
                //};

                //var closeBtn = new Button
                //{
                //    Content = "Cerrar",
                //    Command = dialogViewModel.CloseCommand,
                //    Margin = new Thickness(10)
                //};

                //dialogContent.Children.Add(dialogTextBlock);
                //dialogContent.Children.Add(closeBtn);

                //await DialogHost.Show(dialogContent, "RootDialog");


                //var dialogContent = new StackPanel();

                //var dialogTextBlock = new TextBlock
                //{
                //    Text = "Dynamic Dialog!",
                //    Margin = new Thickness(20)
                //};

                //var closeBtn = new Button
                //{
                //    Content = "Cerrar",
                //    Command = DialogHost.CloseDialogCommand,
                //    CommandParameter = null,
                //    Margin = new Thickness(10)
                //};

                ////var registerBtn = new Button
                ////{
                ////    Content = "Registrar Información",
                ////    Command = new RelayCommand(null),
                ////    Margin = new Thickness(10)
                ////};

                //dialogContent.Children.Add(dialogTextBlock);
                //dialogContent.Children.Add(closeBtn);
                ////dialogContent.Children.Add(registerBtn);

                //await MaterialDesignThemes.Wpf.DialogHost.Show(dialogContent);

                //MessageBox.Show(queue.Number.ToString());
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message.ToString());
                NotifiactionMessage
                    .SetMessage("Error", GlobalMessages.INTERNAL_SERVER_ERROR, NotificationType.Error);
            }


        }

        private void OpenDynamicDialog(object parameter)
        {
            var queue = parameter as CallsInQueues;
            if (queue != null)
            {
                // Aquí puedes realizar cualquier lógica necesaria antes de abrir el diálogo dinámico

                // Por ejemplo, asignando el texto dinámico
                var dialogViewModel = new CallsInQueuesUCViewModel
                {
                    // DialogText = "Dynamic Dialog! " + queue.Number.ToString()
                };

                // Ahora, abre el diálogo utilizando el DialogHost de la vista
                // Puedes utilizar un Messenger o similar para comunicarte con la vista
                Messenger.Default.Send(new NotificationMessage(dialogViewModel, "OpenDynamicDialog"));
            }
        }



        private void ClearFilters()
        {
            SearchByNumber = string.Empty;
            ValueShortText = string.Empty;
            SearchByAttribute = string.Empty;
        }


    }
}
