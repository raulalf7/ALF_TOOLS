using System;
using System.ComponentModel;
using System.ServiceModel;
using ALF.SL.UploadWeb.SilverlightUploadServiceReference;

namespace ALF.SL.UploadWeb.DataModel
{
    /// <summary>
    ///     文件上传类
    /// </summary>
    public class FileUploader
    {
        private readonly SilverlightUploadServiceClient _client;
        private readonly long _dataLength;
        private readonly UserFile _file;
        private long _dataSent;
        private bool _firstChunk = true;
        private string _initParams;
        private bool _lastChunk;

        public FileUploader(UserFile file)
        {
            _file = file;

            _dataLength = _file.FileStream.Length;
            _dataSent = 0;

            ////创建WCF端，此处被注释
            _client = new SilverlightUploadServiceClient( );
            //事件绑定
            _client.StoreFileAdvancedCompleted += _client_StoreFileAdvancedCompleted;
            _client.CancelUploadCompleted += _client_CancelUploadCompleted;
            _client.ChannelFactory.Closed += ChannelFactory_Closed;
        }

        /// <summary>
        ///     上传完成事件处理对象声明
        /// </summary>
        public event EventHandler UploadFinished;

        public void UploadAdvanced(string initParams)
        {
            _initParams = initParams;

            UploadAdvanced();
        }

        /// <summary>
        ///     上传文件
        /// </summary>
        private void UploadAdvanced()
        {
            var buffer = new byte[4*4096];
            var bytesRead = _file.FileStream.Read(buffer, 0, buffer.Length);

            //文件是否上传完毕?
            if (bytesRead != 0)
            {
                if (_client.ChannelFactory.State==CommunicationState.Closed)
                {
   //                 _client.ChannelFactory.Open();
                }


                _dataSent += bytesRead;

                if (_dataSent == _dataLength)
                    _lastChunk = true; //是否是最后一块数据，这样WCF会在服务端根据该信息来决定是否对临时文件重命名

                //上传当前数据块
                _client.StoreFileAdvancedAsync(_file.FileName, buffer, bytesRead, _initParams, _firstChunk, _lastChunk);


                //在第一条消息之后一直为false
                _firstChunk = false;

                //通知上传进度修改
                OnProgressChanged();
            }
            else
            {
                //当上传完毕后
                _file.FileStream.Dispose();
                _file.FileStream.Close();
                ChannelIsClosed();
                //          _client.ChannelFactory.Close();
            }
        }

        /// <summary>
        ///     修改进度属性
        /// </summary>
        private void OnProgressChanged()
        {
            _file.BytesUploaded = _dataSent; //注：此处会先调用FileCollection中的同名属性，然后才是_file.BytesUploaded属性绑定
        }

        private void _client_StoreFileAdvancedCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //检查WEB服务是否存在错误
            if (e.Error != null)
            {
                //当错误时放弃上传
                _file.State = Constants.FileStates.Error;
            }
            else
            {
                //如果文件未取消上传的话，则继续上传
                if (!_file.IsDeleted)
                    UploadAdvanced();
            }
        }

        #region

        /// <summary>
        ///     关闭ChannelFactory事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelFactory_Closed(object sender, EventArgs e)
        {
            ChannelIsClosed();
        }

        private void _client_CancelUploadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //当取消上传完成后关闭Channel
         //   _client.ChannelFactory.Close();
        }

        /// <summary>
        ///     Channel被关闭
        /// </summary>
        private void ChannelIsClosed()
        {
            if (!_file.IsDeleted)
            {
                if (UploadFinished != null)
                    UploadFinished(this, null);
            }
        }

        /// <summary>
        ///     取消上传
        /// </summary>
        public void CancelUpload()
        {
            _client.CancelUploadAsync(_file.FileName);
        }

        #endregion
    }
}