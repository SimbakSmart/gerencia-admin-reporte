

using GalaSoft.MvvmLight;
using Gerencia_Reportes.Helpers;
using Gerencia_Reportes.Interfaces;
using Gerencia_Reportes.Models;
using Gerencia_Reportes.Services;
using Gerencia_Reportes.Utils;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Gerencia_Reportes.ViewModels.UC
{
    public class CallsInQueuesUCViewModel : ViewModelBase
    {
        private IEpicorProvider _service;

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


        public CallsInQueuesUCViewModel()
        {
            _service = new EpicorProvider();

            Task.Run(async() => {
             await LoadDataAsync();
            });
        }

        public async Task LoadDataAsync(string queryParams = "")
        {
            try
            {
                IsLoading = true;

                //Thread.Sleep(5000);

                list = await _service.FetchAllAsync(queryParams);


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
    }

    
}
