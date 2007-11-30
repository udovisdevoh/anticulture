using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AntiCulture.Audio.Midi
{
    public class Midi
    {
        #region Data members
        private List<MidiTrack> mMidiTracks = new List<MidiTrack>();
        private float mBeatsPerMinute = 120.0f;
        private float mEndingSilence = 1.0f;
        #endregion

        #region Private static methods
        static private uint SwapBytes(uint value)
        {
            return (value >> 24) | ((value & 0xFF) << 24) | ((value & 0xFF00) << 8) | ((value & 0xFF0000) >> 8);
        }

        static private ushort SwapBytes(ushort value)
        {
            return (ushort)((((uint)value >> 8) & 0xFF) | (((uint)value & 0xFF) << 8));
        }

        static private void WriteVariableLength(BinaryWriter binaryWriter, uint value)
        {
            if (value < 128)
            {
                binaryWriter.Write((byte)value);
            }
            else if (value < 16384)
            {
                binaryWriter.Write((byte)((value & 0x7F) | (1 << 7)));
                binaryWriter.Write((byte)((value >> 7) & 0x7F));
            }
            else
            {
                throw new NotImplementedException("Big variable length numbers");
            }
        }
        #endregion

        #region Public properties
        public float BeatsPerMinute
        {
            get { return mBeatsPerMinute; }
            set { mBeatsPerMinute = value; }
        }

        public float EndingSilence
        {
            get { return mEndingSilence; }
            set { mEndingSilence = value; }
        }
        #endregion

        #region Public methods
        public void AddTrack(string name, string instrument, IEnumerator<NoteEvent> noteEventEnumerator)
        {
            mMidiTracks.Add(new MidiTrack(name, instrument, noteEventEnumerator));
        }

        public void Write(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                Write(stream);
            }
        }

        public void Write(Stream stream)
        {
            if (mMidiTracks.Count == 0) throw new InvalidOperationException("Not midi tracks registered");
            if (!stream.CanSeek) throw new ArgumentException("Stream must be seekable", "stream");

            BinaryWriter binaryWriter = new BinaryWriter(stream);

            // Write file header
            binaryWriter.Write("MThd".ToCharArray()); // Chunk ID
            binaryWriter.Write(SwapBytes((uint)6)); // Size of chunk
            binaryWriter.Write(SwapBytes((ushort)(mMidiTracks.Count == 1 ? 0 : 1))); // Format type
            binaryWriter.Write(SwapBytes((ushort)mMidiTracks.Count)); // Number of tracks
            binaryWriter.Write(SwapBytes((ushort)64)); // Delta-time ticks per quarter note

            // For each track
            foreach(MidiTrack track in mMidiTracks)
            {
                // Write track header
                binaryWriter.Write("MTrk".ToCharArray()); // Chunk ID
                long trackSizePosition = stream.Position;
                binaryWriter.Write((uint)0); // Zero-sized for the moment of chunk
                long trackBeginPosition = stream.Position;

                // Write tempo
                binaryWriter.Write((byte)0); // Ticks after last event
                binaryWriter.Write((byte)0xFF); // Control command
                binaryWriter.Write((byte)0x51); // Tempo command
                uint microsecondsPerBeat = (uint)((1.0 / (double)mBeatsPerMinute) * 60.0 * 1000.0 * 1000.0);
                binaryWriter.Write(SwapBytes(microsecondsPerBeat | (0x03 << 24))); // Tempo

                // Write note events
                IEnumerator<MidiNoteEvent> midiNoteEventEnumerator = track.GetMidiNoteEventEnumerator();
                uint lastNoteEventTime = 0;
                while (midiNoteEventEnumerator.MoveNext())
                {
                    MidiNoteEvent midiNoteEvent = midiNoteEventEnumerator.Current;

                    WriteVariableLength(binaryWriter, midiNoteEvent.Time - lastNoteEventTime); // Ticks after last event
                    binaryWriter.Write(midiNoteEvent.Type); // Note on/off, channel 0
                    binaryWriter.Write((byte)midiNoteEvent.Note); // Note ID
                    binaryWriter.Write((byte)midiNoteEvent.Velocity); // Note velocity

                    lastNoteEventTime = midiNoteEvent.Time;
                }

                // Write end of track delimiter
                WriteVariableLength(binaryWriter, (uint)(mEndingSilence*64)); // Ticks after last event
                binaryWriter.Write((byte)0xFF); // Control message
                binaryWriter.Write((byte)0x2F); // End of track
                binaryWriter.Write((byte)0); // Required

                // Loop back to track size and write it
                long trackEndPosition = stream.Position;
                stream.Position = trackSizePosition;
                binaryWriter.Write(SwapBytes((uint)(trackEndPosition - trackBeginPosition)));
                stream.Position = trackEndPosition;
            }
        }
        #endregion
    }
}