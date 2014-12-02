using System;
using System.Collections.Generic;



namespace WavDotNet.Core
{
	public partial class SampleConverter<TIn, TOut>
	{
		private readonly AlgoManager<TIn, TOut> manager = new AlgoManager<TIn, TOut>();



		public TOut[] Convert(IEnumerable<TIn> samples)
		{
			if (samples == null) { throw new ArgumentNullException("samples"); }

			var algo = manager.GetAlgo();

			var converted = new List<TOut>();

			foreach (var sample in samples)
			{
				converted.Add(algo(sample));
			}

			return converted.ToArray();
		}

		public TOut Convert(TIn sample)
		{
			return manager.GetAlgo()(sample);
		}
	}
}
