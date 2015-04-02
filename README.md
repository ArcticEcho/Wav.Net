Wav.Net [![Build Status](https://travis-ci.org/ArcticEcho/Wav.Net.svg?branch=master)](https://travis-ci.org/ArcticEcho/Wav.Net)
=======

Wav.Net is a fully featured .Net library<sup>§</sup> for transcoding wave files (reading, writing/creating, etc.).

Audio support:

 - Bit depth: 8 to 64-bits,
 - Sample rate: 1 to 4,294,967,296Hz,
 - Audio format: PCM and IEEE floating-point,
 - Supports `Stream`s? Yes,
 - Max channels: 19,
 - Max file size: 2GiB.

Current features:

 - Tone generators: [Sine][sineGen], [sawtooth][sawGen], [square][sqrGen] & [white noise][whtNoiseGen],
 - Low pass filters: [Linkwitz Riley][linkLP] (4th order) & [Bessel][besselLP] (24dB/Oct),
 - High pass filter: [Linkwitz Riley][linkHP] (4th order),
 - [Phase shifter][phaseShft],
 - [Sample inverter][smpInv],
 - [Decibel/amplitude converter][dBAmpCal],
 - [Audio stream precision calculator][strmPrecCal].
 
---

 <sup>§ *Requires .Net 2.0 or higher, and yes, it's Mono compatible.*</sup>

 [sineGen]: https://github.com/ArcticEcho/Wav.Net/blob/master/Wav.Net/Tools/Generators/SineWave.cs#L34
 [sawGen]: https://github.com/ArcticEcho/Wav.Net/blob/master/Wav.Net/Tools/Generators/Sawtooth.cs#L32
 [sqrGen]: https://github.com/ArcticEcho/Wav.Net/blob/master/Wav.Net/Tools/Generators/SquareWave.cs#L32
 [whtNoiseGen]: https://github.com/ArcticEcho/Wav.Net/blob/master/Wav.Net/Tools/Generators/WhiteNoise.cs#L32
 [linkHP]: https://github.com/ArcticEcho/Wav.Net/blob/master/Wav.Net/Tools/Filters/LinkwitzRileyHighPass.cs#L33
 [linkLP]: https://github.com/ArcticEcho/Wav.Net/blob/master/Wav.Net/Tools/Filters/LinkwitzRileyLowPass.cs#L34
 [besselLP]: https://github.com/ArcticEcho/Wav.Net/blob/master/Wav.Net/Tools/Filters/BesselLowPass.cs#L33
 [phaseShft]: https://github.com/ArcticEcho/Wav.Net/blob/master/Wav.Net/Tools/PhaseShifter.cs#L32
 [smpInv]: https://github.com/ArcticEcho/Wav.Net/blob/master/Wav.Net/Tools/SampleInverter.cs#L25
 [dBAmpCal]: https://github.com/ArcticEcho/Wav.Net/blob/master/Wav.Net/Tools/Math.cs#L29-L60
 [strmPrecCal]: https://github.com/ArcticEcho/Wav.Net/blob/master/Wav.Net/Tools/Math.cs#L62-L121
