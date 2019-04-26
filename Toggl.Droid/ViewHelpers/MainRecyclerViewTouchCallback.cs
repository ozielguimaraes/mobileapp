using System;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using Toggl.Droid.Adapters;
using Toggl.Droid.ViewHolders;

namespace Toggl.Droid.ViewHelpers
{
    public class MainRecyclerViewTouchCallback : ItemTouchHelper.SimpleCallback
    {
        private MainRecyclerAdapter adapter;

        public MainRecyclerViewTouchCallback(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public MainRecyclerViewTouchCallback(MainRecyclerAdapter adapter) : base(0, ItemTouchHelper.Left | ItemTouchHelper.Right)
        {
            this.adapter = adapter;
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
            => false;

        public override int GetSwipeDirs(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            if (viewHolder is MainLogCellViewHolder mainLogCellViewHolder && mainLogCellViewHolder.CanSync)
            {
                return !mainLogCellViewHolder.Item.ViewModel.CanContinue ? ItemTouchHelper.Left : base.GetSwipeDirs(recyclerView, viewHolder);
            }

            return ItemTouchHelper.ActionStateIdle;
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            var swipedPosition = viewHolder.AdapterPosition;

            if (direction == ItemTouchHelper.Right)
            {
                adapter.ContinueTimeEntryBySwiping(swipedPosition);
            }
            else
            {
                adapter.DeleteTimeEntry(swipedPosition);
            }
        }

        public override void OnChildDraw(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
        {
            if (viewHolder is MainLogCellViewHolder logViewHolder)
            {
                if (dX > 0)
                {
                    logViewHolder.ShowSwipeToContinueBackground();
                }
                else if (dX < 0)
                {
                    logViewHolder.ShowSwipeToDeleteBackground();
                }
                else
                {
                    logViewHolder.HideSwipeBackgrounds();
                }

                DefaultUIUtil.OnDraw(c, recyclerView, logViewHolder.MainLogContentView, dX, dY, actionState, isCurrentlyActive);
            }
            else
            {
                base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
            }
        }

        public override void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
        {
            if (viewHolder is MainLogCellViewHolder logViewHolder)
            {
                DefaultUIUtil.OnSelected(logViewHolder.MainLogContentView);
            }
            else
            {
                base.OnSelectedChanged(viewHolder, actionState);
            }
        }

        public override void ClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            if (viewHolder is MainLogCellViewHolder logViewHolder)
            {
                DefaultUIUtil.ClearView(logViewHolder.MainLogContentView);
                logViewHolder.ItemView.ImportantForAccessibility = ImportantForAccessibility.Auto;
            }
            else
            {
                base.ClearView(recyclerView, viewHolder);
            }

            resetViewsImportantForAccessibilityStatus(recyclerView);
        }

        private void resetViewsImportantForAccessibilityStatus(RecyclerView recyclerView)
        {
            var lm = recyclerView.GetLayoutManager();
            if (lm is null) return;
            var childCount = lm.ChildCount;

            for (var layoutIndex = 0; layoutIndex < childCount; layoutIndex++)
            {
                var child = recyclerView.FindViewHolderForLayoutPosition(layoutIndex);
                if (child == null || child.ItemView.ImportantForAccessibility == ImportantForAccessibility.Auto) 
                    continue;

                child.ItemView.ImportantForAccessibility = ImportantForAccessibility.Auto;
            }
        }
    }
}
