#region Copyright
/*
* Copyright (c) 2005-2010, OpenMI Association
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the OpenMI Association nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "OpenMI Association" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "OpenMI Association" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3603
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class opr 
{
    
    private oprModel[] modelsField;
    
    private oprConnection[] connectionsField;
    
    private string versionField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("model", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public oprModel[] models
    {
        get 
        {
            return this.modelsField;
        }
        set 
        {
            this.modelsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("connection", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public oprConnection[] connections
    {
        get 
        {
            return this.connectionsField;
        }
        set
        {
            this.connectionsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string version
    {
        get
        {
            return this.versionField;
        }
        set 
        {
            this.versionField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class oprModel 
{
    
    private string omiField;
    
    private string rect_xField;
    
    private string rect_yField;
    
    private string rect_widthField;
    
    private string rect_heightField;
    
    private bool is_triggerField;
    
    public oprModel() 
    {
        this.is_triggerField = false;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string omi
    {
        get 
        {
            return this.omiField;
        }
        set 
        {
            this.omiField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string rect_x 
    {
        get 
        {
            return this.rect_xField;
        }
        set
        {
            this.rect_xField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string rect_y 
    {
        get
        {
            return this.rect_yField;
        }
        set 
        {
            this.rect_yField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string rect_width
    {
        get 
        {
            return this.rect_widthField;
        }
        set 
        {
            this.rect_widthField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string rect_height 
    {
        get
        {
            return this.rect_heightField;
        }
        set 
        {
            this.rect_heightField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    [System.ComponentModel.DefaultValueAttribute(false)]
    public bool is_trigger 
    {
        get 
        {
            return this.is_triggerField;
        }
        set 
        {
            this.is_triggerField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class oprConnection 
{
    private oprConnectionLink[] linksField;
    
    private oprConnectionDecorator[] decoratorsField;

    private int source_model_indexField;

    private bool source_model_indexFieldSpecified;
    
    private int target_model_indexField;
    
    private bool target_model_indexFieldSpecified;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("link", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public oprConnectionLink[] links 
    {
        get 
        {
            return this.linksField;
        }
        set 
        {
            this.linksField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("adapted_outputs", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public oprConnectionDecorator[] decorators 
    {
        get 
        {
            return this.decoratorsField;
        }
        set 
        {
            this.decoratorsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int source_model_index
    {
        get 
        {
            return this.source_model_indexField;
        }
        set
        {
            this.source_model_indexField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool source_model_indexSpecified 
    {
        get 
        {
            return this.source_model_indexFieldSpecified;
        }
        set 
        {
            this.source_model_indexFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int target_model_index 
    {
        get 
        {
            return this.target_model_indexField;
        }
        set 
        {
            this.target_model_indexField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool target_model_indexSpecified 
    {
        get 
        {
            return this.target_model_indexFieldSpecified;
        }
        set 
        {
            this.target_model_indexFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class oprConnectionLink
{
    
    private oprConnectionLinkDecorated[] decoratedField;

    private oprSourceItem[] source_item_idFields;

    private oprSourceItem source_item_idField;
    
    private string target_item_idField;
    
    /// <remarks/>
    //[System.Xml.Serialization.XmlElementAttribute("decorated", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    ///public oprConnectionLinkDecorated[] decorated 
    //{
    //    get
    //    {
    //        return this.decoratedField;
    //    }
    //    set 
    //    {
    //        this.decoratedField = value;
    //    }
    //}
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public oprSourceItem source_item_id 
    {
        get 
        {
            return this.source_item_idField;
        }

        set 
        {
            this.source_item_idField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute("multi_input_exchangeitem_sources", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("source", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public oprSourceItem[] source_item_ids
    {
        get
        {
            return this.source_item_idFields;
        }

        set
        {
            this.source_item_idFields = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string target_item_id 
    {
        get 
        {
            return this.target_item_idField;
        }

        set 
        {
            this.target_item_idField = value;
        }
    }
}


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class oprSourceItem
{
    private string source_item_idField;

    private bool source_item_is_adaptedoutput_field;

    private int source_item_adpatedoutput_index_field;

    private oprSourceItem parent_item_idfield;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string source_item_id
    {
        get
        {
            return this.source_item_idField;
        }

        set
        {
            this.source_item_idField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public oprSourceItem parent_item_id
    {
        get
        {
            return this.parent_item_idfield;
        }

        set
        {
            this.parent_item_idfield = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool source_item_is_adaptedoutput
    {
        get
        {
            return this.source_item_is_adaptedoutput_field;
        }

        set
        {
            this.source_item_is_adaptedoutput_field = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int source_item_adpatedoutput_index
    {
        get
        {
            return this.source_item_adpatedoutput_index_field;
        }

        set
        {
            this.source_item_adpatedoutput_index_field = value;
        }
    }

}



/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class oprConnectionLinkDecorated 
{
    
    private int indexField;
    
    private bool indexFieldSpecified;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int index 
    {
        get 
        {
            return this.indexField;
        }

        set 
        {
            this.indexField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool indexSpecified 
    {
        get 
        {
            return this.indexFieldSpecified;
        }
        
        set 
        {
            this.indexFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class oprConnectionDecorator 
{
    private oprConnectionDecoratorFactory factoryField;
    
    private oprConnectionDecoratorArgument[] argumentsField;
    
    private string item_idField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public oprConnectionDecoratorFactory factory 
    {
        get 
        {
            return this.factoryField;
        }
        set 
        {

            this.factoryField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("argument", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public oprConnectionDecoratorArgument[] arguments 
    {
        get 
        {
            return this.argumentsField;
        }
        set 
        {
            this.argumentsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string item_id 
    {
        get 
        {
            return this.item_idField;
        }
        set 
        {
            this.item_idField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class oprConnectionDecoratorFactory
{
    
    private string idField;
    
    private string assemblyField;
    
    private string typeField;
    
    public oprConnectionDecoratorFactory() 
    {
        this.assemblyField = "SourceComponent";
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id 
    {
        get 
        {
            return this.idField;
        }
        
        set 
        {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    [System.ComponentModel.DefaultValueAttribute("SourceComponent")]
    public string assembly 
    {
        get 
        {
            return this.assemblyField;
        }
        
        set 
        {
            this.assemblyField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string type 
    {
        get 
        {
            return this.typeField;
        }
        
        set 
        {
            this.typeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class oprConnectionDecoratorArgument 
{
    
    private string idField;
    
    private string valueField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id 
    {
        get 
        {
            return this.idField;
        }

        set 
        {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string value 
    {
        get 
        {
            return this.valueField;
        }

        set 
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class NewDataSet 
{
    
    private opr itemField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("opr")]
    public opr Item 
    {
        get 
        {
            return this.itemField;
        }
        set 
        {
            this.itemField = value;
        }
    }
}
