using System;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toggl.Core;
using Toggl.Core.UI.ViewModels;

namespace Toggl.Droid.ViewHolders
{
    public sealed class TagCreationSelectionViewHolder : BaseRecyclerViewHolder<SelectableTagBaseViewModel>
    {
        private TextView creationTextView;

        public TagCreationSelectionViewHolder(View itemView) : base(itemView)
        {
        }

        public TagCreationSelectionViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            creationTextView = ItemView.FindViewById<TextView>(Resource.Id.CreationLabel);
        }

        protected override void UpdateView()
        {
            creationTextView.Text = $"{Resources.CreateTag} \"{Item.Name.Trim()}\"";
        }
    }
}
