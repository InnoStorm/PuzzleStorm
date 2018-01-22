﻿using System;
using System.Windows;
using System.Windows.Input;

namespace Client {
    class WindowViewModel : BaseViewModel {

        #region Private

        private Window mWindow;

        #endregion

        #region Properties

        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.MainPage;

        #endregion

        #region Command

        public ICommand CloseWindowCommand { get; set;  }

        #endregion

        #region Constructors

        public WindowViewModel(Window window)
        {
            this.mWindow = window;

            CloseWindowCommand = new RelayCommand(DisposeRabbitBus);
        }

        #endregion

        #region Metods

        private void DisposeRabbitBus() {   
            if(RabbitBus.Instance.Bus != null && RabbitBus.Instance.Bus.IsConnected)
                RabbitBus.Instance.Bus.Dispose();
        }

        #endregion
    }
}
