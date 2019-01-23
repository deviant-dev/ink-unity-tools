using Deviant.Utils;

namespace Deviant.Commands {
	public class GenericBucketEditor<TBucket, TItem> : Editor<TBucket> where TBucket : GenericBucket<TItem> where TItem : class { }
}