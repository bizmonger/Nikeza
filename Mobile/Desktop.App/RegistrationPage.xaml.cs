﻿using System.Windows.Controls;
using Nikeza.Mobile.UILogic.Registration;

namespace Desktop.App
{
    public partial class RegistrationPage : Page
    {
        ViewModel _viewmodel;

        public RegistrationPage()
        {
            InitializeComponent();
            
            _viewmodel = new ViewModel(FunctionFactory.SubmitRegistration());

            Password.PasswordChanged += (s, e) =>
                {
                    _viewmodel.Password = Password.Password;
                    _viewmodel.Validate.Execute(null);
                };

            Confirm.PasswordChanged += (s, e) =>
                {
                    _viewmodel.Confirm = Confirm.Password;
                    _viewmodel.Validate.Execute(null);
                };

            DataContext = _viewmodel;
        }
    }
}