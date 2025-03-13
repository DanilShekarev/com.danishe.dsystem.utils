using DSystemUtils.Reflection;
using JetBrains.Annotations;

namespace DSystemUtils.Dynamic.CDS
{
    public static class DCSExtensions
    {
        public static void Attach<T>([NotNull] this IDataController<T> dataController, [NotNull] T data) where T : class, IDynamicData
        {
            data.TryDisposeSubscribe((sender, args) =>
            {
                dataController.Detach(sender as T);
            });
            dataController.AttachInternal(data);
        }

        public static void Attach([NotNull] this IDataController dataController, [NotNull] IDynamicData data)
        {
            data.TryDisposeSubscribe((sender, args) =>
            {
                dataController.Detach(sender as IDynamicData);
            });
            dataController.AttachInternal(data);
        }
        
        public static void Detach<T>([NotNull] this IDataController<T> dataController, [NotNull] T data) where T : IDynamicData
        {
            dataController.DetachInternal(data);
        }
        
        public static void Detach([NotNull] this IDataController dataController, [NotNull] IDynamicData data)
        {
            dataController.DetachInternal(data);
        }
    }
}