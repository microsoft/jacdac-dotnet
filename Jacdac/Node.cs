﻿using System;

namespace Jacdac
{
    public delegate void NodeEventHandler(JDNode sender, EventArgs e);

    public abstract class JDNode
    {
        internal JDNode()
        {

        }

        public event NodeEventHandler Changed;

        protected void RaiseChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    public abstract class JDServiceNode : JDNode
    {
        public readonly JDService Service;
        public readonly ushort Code;

        internal JDServiceNode(JDService service, ushort code)
        {
            this.Service = service;
            this.Code = code;
        }
    }
}