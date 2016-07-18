using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CredentialType
/// </summary>
public class CredentialType
{
    public int CredentialTypeID {get; set;}
    public string CredentialTypeName {get; set;}
    public string Description {get; set;}

	public CredentialType()
	{
	}

    public CredentialType(
        int credentialTypeID,
        string credentialTypeName,
        string description)
    {
        this.CredentialTypeID = credentialTypeID;
        this.CredentialTypeName = credentialTypeName;
        this.Description = description;

    }

}