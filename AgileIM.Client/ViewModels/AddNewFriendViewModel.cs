﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Agile.Client.Service.Services;

using AgileIM.Client.Models;
using AgileIM.Shared.Models.ApiResult;
using AgileIM.Shared.Models.Users.Dto;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace AgileIM.Client.ViewModels
{
    public class AddNewFriendViewModel : ObservableObject
    {
        public AddNewFriendViewModel(IUserService userService)
        {
            _userService = userService;
        }

        #region Service
        private readonly IUserService _userService;
        #endregion

        #region Property
        private string _searchParamText;
        private List<UserInfoDto> _searchUserInfoList;

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchParamText
        {
            get => _searchParamText;
            set => SetProperty(ref _searchParamText, value);
        }
        /// <summary>
        /// 搜索用户列表
        /// </summary>
        public List<UserInfoDto> SearchUserInfoList
        {
            get => _searchUserInfoList;
            set => SetProperty(ref _searchUserInfoList, value);
        }
        #endregion

        #region Command
        public ICommand SearchCommand => new AsyncRelayCommand(SearchAsync);
        #endregion

        #region Methodes
        /// <summary>
        /// 搜索
        /// </summary>
        private async Task SearchAsync()
        {
            if (string.IsNullOrEmpty(SearchParamText?.Trim()))
            {
                return;
            }

            var result = await _userService.QueryFriends(SearchParamText);
            if (result is not null && result.Code.Equals(200))
                SearchUserInfoList = new List<UserInfoDto>(result.Data);
            else
                SearchUserInfoList = new List<UserInfoDto>();
        }
        #endregion



    }
}