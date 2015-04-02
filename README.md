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

 - Tone generators: Sine, sawtooth, square & white noise,
 - Low pass filters: Linkwitz Riley (4th order) & Bessel (24dB/Oct),
 - High pass filter: Linkwitz Riley (4th order),
 - Phase shifter,
 - Sample inverter,
 - Decibel/amplitude converter,
 - Audio stream precision calculator.
 
---

 <sup>§ *Requires .Net 2.0 or higher, and yes, it's Mono compatible.*</sup>
