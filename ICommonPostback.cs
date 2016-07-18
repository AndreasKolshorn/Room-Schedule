using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel; 



/// <summary>
/// Summary description for ICommonPostback
/// </summary>
public interface ICommonPostback
{
  int IItemID 
  { get; set; } 
  int IClientUploadID 
  { get; set; } 
}