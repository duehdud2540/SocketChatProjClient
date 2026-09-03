using Grpc.Net.Client;
using SocketChatCllient.ViewModel;
using SocketChatServer.Protos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Net.WebRequestMethods;

namespace SocketChatClient.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly string _serverAddress = "https://localhost:7103";//Environment.GetEnvironmentVariable("SocketChatServerPort") ?? "";
                                                                            // ㅅㅂ 이건 왜안되는거야
        private readonly Action<ViewModelBase> _navigate;
        private string _userId = string.Empty;
        public string UserId
        {
            get => _userId;
            set { _userId = value; OnPropertyChanged(); }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }
        public ICommand LoginCommand { get; }

        public LoginViewModel(Action<ViewModelBase> navigate)
        {
            _navigate = navigate;
            LoginCommand = new RelayCommand(async (param) => await ExecuteLoginAsync(param));
        }

        //일단 로그인 버튼 클릭 시 메서드
        private async Task ExecuteLoginAsync(object? parameter)
        {
            var passwordBox = parameter as PasswordBox;
            string password = passwordBox?.Password ?? string.Empty;
            if (string.IsNullOrWhiteSpace(UserId) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "아이디와 비밀번호를 모두 입력해주세요.";
                return;
            }
            ErrorMessage = "로그인 중...";
            try
            {
                var httpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                using var channel = GrpcChannel.ForAddress(_serverAddress, new GrpcChannelOptions
                {
                    HttpHandler = httpHandler
                });
                var client = new AuthService.AuthServiceClient(channel);
                var request = new LoginRequest
                {
                    UserId = UserId,
                    Password = password
                };

                var response = await client.LoginAsync(request);

                if (response.IsSuccess)
                {
                    ErrorMessage = string.Empty;
                    _navigate(new ChatViewModel());
                }
                else
                {
                    ErrorMessage = string.IsNullOrEmpty(response.Message)
                        ? "아이디 또는 비밀번호가 일치하지 않습니다."
                        : response.Message;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[gRPC Error] {ex}");
                ErrorMessage = $"서버 연결 실패: {ex.Message}";
            }
        }

        // 회원가입
        private async Task ExecuteJoinAsync(object? parameter)
        {
            return;
        }

        //아이디 패스워드 찾기
        private async Task ID_PasswordFindAsync(object? parameter)
        {
            return;
        }
    }
}