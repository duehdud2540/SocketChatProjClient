using System;
using System.Collections.Generic;
using System.Text;

namespace SocketChatClient.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            // 앱 실행 시 첫 화면을 로그인 화면으로 세팅
            CurrentViewModel = new LoginViewModel();
        }
    }
}
