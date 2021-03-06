﻿namespace SoundFingerprinting.Audio.Bass
{
    using System.Collections.Generic;

    using SoundFingerprinting.Infrastructure;

    /// <summary>
    ///   Bass Audio Service
    /// </summary>
    /// <remarks>
    ///   BASS is an audio library for use in Windows and Mac OSX software. 
    ///   Its purpose is to provide developers with powerful and efficient sample, stream (MP3, MP2, MP1, OGG, WAV, AIFF, custom generated, and more via add-ons), 
    ///   MOD music (XM, IT, S3M, MOD, MTM, UMX), MO3 music (MP3/OGG compressed MODs), and recording functions. 
    /// </remarks>
    public class BassAudioService : AudioService
    {
        private static readonly IReadOnlyCollection<string> BaasSupportedFormats = new[] { ".wav", ".mp3", ".ogg", ".flac" };

        private readonly IBassServiceProxy proxy;
        private readonly IBassStreamFactory streamFactory;
        private readonly IBassResampler resampler;

        public BassAudioService()
            : this(DependencyResolver.Current.Get<IBassServiceProxy>(), DependencyResolver.Current.Get<IBassStreamFactory>(), DependencyResolver.Current.Get<IBassResampler>())
        {
        }

        internal BassAudioService(IBassServiceProxy proxy, IBassStreamFactory streamFactory, IBassResampler resampler)
        {
            this.proxy = proxy;
            this.streamFactory = streamFactory;
            this.resampler = resampler;
        }

        public override IReadOnlyCollection<string> SupportedFormats
        {
            get
            {
                return BaasSupportedFormats;
            }
        }

        public override AudioSamples ReadMonoSamplesFromFile(string pathToSourceFile, int sampleRate, int seconds, int startAt)
        {
            int stream = streamFactory.CreateStream(pathToSourceFile);
            float[] samples = resampler.Resample(stream, sampleRate, seconds, startAt, mixerStream => new BassSamplesProvider(proxy, mixerStream));
            return new AudioSamples
                {
                    Samples = samples,
                    Origin = pathToSourceFile,
                    SampleRate = sampleRate,
                    Duration = (double)samples.Length / sampleRate
                };
        }
    }
}
