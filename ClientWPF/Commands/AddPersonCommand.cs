﻿using ClientWPF.Services;
using ClientWPF.Stores;
using ClientWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientWPF.Commands
{
    public class AddPersonCommand : CommandBase
    {
        private readonly AddPersonViewModel _addPersonViewModel;
        private readonly PeopleStore _peopleStore;
        private readonly INavigationService _navigationService;

        public AddPersonCommand(AddPersonViewModel addPersonViewModel, PeopleStore peopleStore, INavigationService navigationService)
        {
            _addPersonViewModel = addPersonViewModel;
            _peopleStore = peopleStore;
            _navigationService = navigationService;
        }

        public override void Execute(object parameter)
        {
            string name = _addPersonViewModel.Name;
            _peopleStore.AddPerson(name);

            _navigationService.Navigate();
        }
    }
}
