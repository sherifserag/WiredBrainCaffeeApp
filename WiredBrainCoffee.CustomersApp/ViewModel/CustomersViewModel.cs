using Accessibility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WiredBrainCoffee.CustomersApp.Command;
using WiredBrainCoffee.CustomersApp.Data;
using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.ViewModel
{
	public class CustomersViewModel : ViewModelBase
	{
		private readonly ICustomerDataProvider customerDataProvider;
		private CustomerItemViewModel? _selectedCustomer;
		private NavigationSide _navigationSide;
		public DelegateCommand AddCommand { get; }
		public DelegateCommand MoveNavigationCommand { get; }
		public DelegateCommand DeleteCommand { get; }

		public CustomersViewModel(ICustomerDataProvider customerDataProvider)
        {
            
			this.customerDataProvider = customerDataProvider;
			AddCommand = new DelegateCommand(Add);
			MoveNavigationCommand = new DelegateCommand(MoveNavigation);
			DeleteCommand = new DelegateCommand(Delete,CanDelete);

		}

		

		public ObservableCollection<CustomerItemViewModel> Customers { get; } = new();

		public CustomerItemViewModel? SelectedCustomer {
			get => _selectedCustomer;
			set {
				_selectedCustomer = value;
			    RaisePropertyChanged();
				RaisePropertyChanged(nameof(IsCustomerSelected));
				DeleteCommand.RaiseCanExecuteChanged();
			} 
		}

		public NavigationSide navigationSide {
			get => _navigationSide;
			private set {
				_navigationSide = value; 
				RaisePropertyChanged();
			} }

		public bool IsCustomerSelected => SelectedCustomer is not null;

        public event PropertyChangedEventHandler? PropertyChanged;

		public async override Task LoadAsync() 
		{
		 if (Customers.Any())
		 {
			return;
		 }

		 var customers = await customerDataProvider.GetAllAsync();

			if (customers is not null) {
				foreach (var customer in customers)
				{

					Customers.Add(new CustomerItemViewModel(customer));
				}
			}
			
		
		}

		private void Add(object? parameter)
		{
			var Customer = new Customer { FirstName = "New" };
			var viewModel = new CustomerItemViewModel(Customer);
			Customers.Add(viewModel);
			SelectedCustomer= viewModel;
		}

		private void MoveNavigation(object? parameter)
		{
			navigationSide = navigationSide == 
				NavigationSide.Left ? 
				NavigationSide.Right : 
				NavigationSide.Left;
		}
		private void Delete(object? parameter)
		{
			if (SelectedCustomer is not null)
			{
				Customers.Remove(SelectedCustomer);
				SelectedCustomer = null;
			}
		}
		private bool CanDelete(object? parameter) => SelectedCustomer is not null;	
		

		

		public enum NavigationSide
		{
		Left,
		Right
		}


	}
}
