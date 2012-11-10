namespace ConsoleTorrent
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Represents a collection of the best trackers.
    /// </summary>
    public class TopListeners : TraceListener
    {
        /// <summary>
        /// Capacity of the traces collection.
        /// </summary>
        private int capacity;

        /// <summary>
        /// Collection of traces.
        /// </summary>
        private LinkedList<string> traces;

        /// <summary>
        /// Initializes a new instance of the <see cref="TopListeners" /> class with the given capacity.
        /// </summary>
        /// <param name="capacity">New capacity for the traces collection.</param>
        public TopListeners(int capacity)
        {
            this.capacity = capacity;
            this.traces = new LinkedList<string>();
        }

        /// <summary>
        /// Concatenates last trace's value with the given message.
        /// </summary>
        /// <param name="message">Message to be concatenated.</param>
        public override void Write(string message)
        {
            lock (traces)
            {
                traces.Last.Value += message;
            }
        }

        /// <summary>
        /// Adds a new trace at the end of the collection, removing the first one if capacity was reached.
        /// </summary>
        /// <param name="message">Message to be added.</param>
        public override void WriteLine(string message)
        {
            lock (traces)
            {
                if (traces.Count >= capacity)
                {
                    traces.RemoveFirst();
                }

                traces.AddLast(message);
            }
        }

        /// <summary>
        /// Writes the collection of traces to the given output stream.
        /// </summary>
        /// <param name="output">Output stream for the collection of traces.</param>
        public void ExportTo(TextWriter output)
        {
            lock (traces)
            {
                foreach (string s in this.traces)
                {
                    output.WriteLine(s);
                }
            }
        }
    }
}
