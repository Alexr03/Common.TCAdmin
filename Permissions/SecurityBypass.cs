using System;
using System.Collections.Generic;
using System.Linq;
using TCAdmin.SDK.Objects;

namespace Alexr03.Common.TCAdmin.Permissions
{
    public class SecurityBypass : IDisposable
    {
        private List<ObjectBase> _objectBases;

        public SecurityBypass()
        {
            ObjectBase.GlobalSkipSecurityCheck = false;
        }

        public SecurityBypass(bool globalSkip = true)
        {
            ObjectBase.GlobalSkipSecurityCheck = globalSkip;
        }
        
        public SecurityBypass(params ObjectBase[] objectBases)
        {
            this._objectBases = objectBases.ToList();
            foreach (var @base in objectBases)
            {
                @base.SkipSecurityCheck = true;
            }
        }

        public void BypassFor(params ObjectBase[] objectBases)
        {
            foreach (var objectBase in objectBases)
            {
                objectBase.SkipSecurityCheck = true;
                _objectBases.Add(objectBase);
            }
        }
        
        public void UnBypassFor(params ObjectBase[] objectBases)
        {
            foreach (var objectBase in objectBases.Where(x => _objectBases.Contains(x)))
            {
                objectBase.SkipSecurityCheck = false;
            }

            _objectBases = _objectBases.Except(objectBases).ToList();
        }

        public void Dispose()
        {
            ObjectBase.GlobalSkipSecurityCheck = false;
            if (_objectBases == null) return;
            
            foreach (var objectBase in _objectBases)
            {
                objectBase.SkipSecurityCheck = false;
            }
        }
    }
}