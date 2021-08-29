using System;
using System.Threading;
using System.Threading.Tasks;
using FileChronicles.Events;
using FileChronicles.InMemoryFileSystem;
using FunkyBasics.Either;

namespace FileChronicles
{
    /// <summary>
    /// the object that keeps accounts of events through the history of a transaction
    /// </summary>
    public class Chronicler : IAsyncDisposable
    {
        private readonly Chronicle _chronicle;
        private readonly FileManager _inMemoryFileSystem;

        /// <summary>
        /// Keep Chronicler from being instantiated directly
        /// </summary>
        private Chronicler() 
        {
            _chronicle = new Chronicle();
            _inMemoryFileSystem = new FileManager();
        }

        /// <summary>
        /// Begins a new Chronicler that will track this Chonicle
        /// </summary>
        /// <returns>a new Chronicler</returns>
        public static Chronicler Begin() => new Chronicler();

        /// <summary>
        /// Commits all recorded events in the chronicle tracked by this Chronicler
        /// </summary>
        /// <returns></returns>
        public async Task<EitherResult<int, ErrorCode>> Commit()
        {
            return await _chronicle.Commit();
        }

        /// <summary>
        /// forgets uncommitted events and rollsback committed events in the chronicle tracked by this Chronicler
        /// </summary>
        /// <returns></returns>
        public async Task Rollback()
        {
            await _chronicle.Rollback();
        }

        /// <summary>
        /// Creates a file with contents
        /// </summary>
        /// <param name="fileName">The location where the file will be made</param>
        /// <param name="bytes">the contents of the file</param>
        /// <param name="cancellationToken">to cancel writing the file</param>
        /// <returns></returns>
        public async Task<EitherResult<EventInfo, ErrorCode>> Create(string fileName, byte[] bytes, CancellationToken cancellationToken = default)
        {
            var createFileEvent = new CreateFileEvent(fileName, bytes, _inMemoryFileSystem, cancellationToken);
            return await _chronicle.AddEvent(createFileEvent);

        }

        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="fileName">the file to be deleted</param>
        /// <param name="cancellationToken">to cancel deleting the file</param>
        /// <returns></returns>
        public async Task<EitherResult<EventInfo, ErrorCode>> Delete(string fileName, CancellationToken cancellationToken = default)
        {
            return await _chronicle.AddEvent(new DeleteFileEvent(fileName, _inMemoryFileSystem, cancellationToken));
        }

        /// <summary>
        /// Moves a file from one location to another
        /// </summary>
        /// <param name="sourceFileName">full file path to the file that is to be moved</param>
        /// <param name="destinationFileName">the location of the desitnation file</param>
        /// <param name="cancellationToken">cancel the move</param>
        /// <returns></returns>
        public async Task<EitherResult<EventInfo, ErrorCode>> Move(string sourceFileName, string destinationFileName, CancellationToken cancellationToken = default)
        {
            return await _chronicle.AddEvent(new MoveFileEvent(sourceFileName, destinationFileName, _inMemoryFileSystem, cancellationToken));
        }

        /// <summary>
        /// Properly disposes of asynchronous resources
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_chronicle != null)
            {
                await _chronicle.Rollback();
            }
        }
    }
}
