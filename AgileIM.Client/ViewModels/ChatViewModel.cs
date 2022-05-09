﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AgileIM.Client.Models;

using System.Windows.Input;
using AgileIM.Client.Controls;
using AgileIM.Client.Views;
using AgileIM.Shared.Models.Users.Dto;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace AgileIM.Client.ViewModels
{
    public class ChatViewModel : ObservableObject, IRecipient<UserInfoDto>
    {

        public ChatViewModel()
        {
            var messages = new List<MessageDto>();
            var messages1 = new List<MessageDto>();
            for (int i = 0; i < 100; i++)
            {
                messages.Add(new MessageDto() { IsSelf = i % 2 == 0, Text = $"消息发送{i}！！！" });
                messages1.Add(new MessageDto() { IsSelf = i % 2 == 0, Text = $"飞翔的企鹅测试消息发送是否成功{i}！！！" });
            }

            ChatUserList.Add(new UserInfoDto { Id = Guid.NewGuid().ToString(), Account = "xwang1234", Nick = "自然醒12", Gender = 1, Messages = new ObservableCollection<MessageDto>(messages), UserNote = "自然醒" });
            ChatUserList.Add(new UserInfoDto { Id = Guid.NewGuid().ToString(), Account = "flay1234", Nick = "飞翔的企鹅", Gender = 1, Messages = new ObservableCollection<MessageDto>(messages1), UserNote = "腼腆的企鹅" });

            WeakReferenceMessenger.Default.Register(this, "ChatViewModel");
        }

        #region Property
        private ObservableCollection<UserInfoDto> _chatUserList = new();
        private UserInfoDto? _selectedUserInfo;
        private bool _sendTextIsFocus;
        private string _sendText;


        /// <summary>
        /// 好友列表
        /// </summary>
        public ObservableCollection<UserInfoDto> ChatUserList
        {
            get => _chatUserList;
            set => SetProperty(ref _chatUserList, value);
        }
        /// <summary>
        /// 当前选中的user
        /// </summary>
        public UserInfoDto? SelectedUserInfo
        {
            get => _selectedUserInfo;
            set
            {
                SetProperty(ref _selectedUserInfo, value);
                OnSelectedUserInfo();
            }
        }
        /// <summary>
        /// 发送消息文本框焦点
        /// </summary>
        public bool SendTextIsFocus
        {
            get => _sendTextIsFocus;
            set => SetProperty(ref _sendTextIsFocus, value);
        }
        /// <summary>
        /// 消息文本框
        /// </summary>
        public string SendText
        {
            get => _sendText;
            set => SetProperty(ref _sendText, value);
        }
        #endregion

        #region Command
        public ICommand SendMessageCommand => new AsyncRelayCommand(SendMessage);
        public ICommand CreateChatCommand => new AsyncRelayCommand(CreateCha);


        #endregion

        #region Methods
        /// <summary>
        /// 选中用户
        /// </summary>
        private void OnSelectedUserInfo()
        {
            SendTextIsFocus = true;
            SelectedUserInfo.IsUnreadMessage = false;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <returns></returns>
        private async Task SendMessage()
        {
            if (SelectedUserInfo is null) return;

            SelectedUserInfo.Messages ??= new ObservableCollection<MessageDto>();

            foreach (var messageDto in SelectedUserInfo.Messages)
                messageDto.IsRead = true;
            if (string.IsNullOrEmpty(SendText)) return;

            SelectedUserInfo.Messages.Add(new MessageDto { Text = SendText, IsSelf = true });
            SendText = string.Empty;
        }

        private Task CreateCha()
        {
            DialogHostHelper.ShowDialog(new CreateChatView());

            return Task.CompletedTask;
        }

        #endregion

        public void Receive(UserInfoDto message)
        {
            var user = ChatUserList.FirstOrDefault(a => a.Id.Equals(message.Id));
            if (user is null)
            {
                ChatUserList.Add(message);
                SelectedUserInfo = message;
            }
            SendTextIsFocus = true;
        }
    }
}
