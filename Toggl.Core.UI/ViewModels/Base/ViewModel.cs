using System.Reactive;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Views;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels
{
    public abstract class ViewModel<TInput, TOutput> : IViewModel
    {
        private readonly INavigationService navigationService;

        public IView View { get; set; }

        public TaskCompletionSource<TOutput> CloseCompletionSource { get; set; }

        protected ViewModel(INavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            this.navigationService = navigationService;
        }

        public Task<TNavigationOutput> Navigate<TViewModel, TNavigationInput, TNavigationOutput>(TNavigationInput payload)
            where TViewModel : ViewModel<TNavigationInput, TNavigationOutput>
        {
            Ensure.Argument.IsNotNull(View, nameof(View));

            return navigationService.Navigate<TViewModel, TNavigationInput, TNavigationOutput>(payload, View);
        }

        public Task Navigate<TViewModel>()
            where TViewModel : ViewModel<Unit, Unit>
            => Navigate<TViewModel, Unit, Unit>(Unit.Default);

        public Task<TNavigationOutput> Navigate<TViewModel, TNavigationOutput>()
            where TViewModel : ViewModel<Unit, TNavigationOutput>
            => Navigate<TViewModel, Unit, TNavigationOutput>(Unit.Default);

        public Task Navigate<TViewModel, TNavigationInput>(TNavigationInput payload)
            where TViewModel : ViewModel<TNavigationInput, Unit>
            => Navigate<TViewModel, TNavigationInput, Unit>(payload);

        public virtual Task Initialize(TInput payload)
            => Task.CompletedTask;

        public async Task Finish(TOutput output)
        {
            await View.Close();
            CloseCompletionSource.SetResult(output);
        }

        public async Task Cancel()
        {
            await View.Close();
            CloseCompletionSource.SetCanceled();
        }

        public void AttachView(IView viewToAttach)
        {
            View = viewToAttach;
        }

        public void DetachView()
        {
            View = null;
        }

        public virtual void ViewAppeared()
        {
        }

        public virtual void ViewAppearing()
        {
        }

        public virtual void ViewDisappearing()
        {
        }

        public virtual void ViewDisappeared()
        {
        }

        public virtual void ViewDestroyed()
        {
        }
    }

    public abstract class ViewModel : ViewModel<Unit, Unit>
    {
        protected ViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public Task Finish()
            => Finish(Unit.Default);

        public virtual Task Initialize()
            => Task.CompletedTask;

        public sealed override Task Initialize(Unit payload)
            => Initialize();
    }
}
