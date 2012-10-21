using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace auto
{
    class ImageFileGallery : IEnumerable<Image<Bgr, Byte>>
    {
        private readonly string[] _dirContent;
        private int _idx = 0;

        public ImageFileGallery(string directory)
        {
            _dirContent = Directory.GetFiles(directory);
        }

        public Image<Bgr, byte> GetImage()
        {
            return new Image<Bgr, byte>(_dirContent[_idx]);
        }

        public void MoveRight()
        {
            _idx = (_idx + 1) % _dirContent.Length;
        }

        public void MoveLeft()
        {
            _idx = _idx == 0 ? _dirContent.Length - 1 : _idx - 1;
        }

        public IEnumerator<Image<Bgr, byte>> GetEnumerator()
        {
            return new ImageFileEnumerator(_dirContent);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class ImageFileEnumerator : IEnumerator<Image<Bgr, byte>>
        {
            private int _idx = 0;
            private readonly string[] _dirContent;

            public ImageFileEnumerator(string[] dirContent)
            {
                _dirContent = dirContent;
            }

            public void Dispose()
            {
                return;
            }

            public bool MoveNext()
            {
                _idx++;
                return _idx < _dirContent.Length;
            }

            public void Reset()
            {
                _idx = 0;
            }

            public Image<Bgr, byte> Current
            {
                get { return new Image<Bgr, byte>(_dirContent[_idx]); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
