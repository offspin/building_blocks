using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;

namespace PartyService
{
    [ServiceContract]
    [XmlSerializerFormat]
    public interface IStaticItemService
    {
        [OperationContract]
        [WebGet(UriTemplate = "{fileName}.{extension}")]
        Stream GetStaticFile(string fileName, string extension);
    }
}
