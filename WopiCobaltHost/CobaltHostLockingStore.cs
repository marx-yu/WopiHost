// Copyright 2014 The Authors Marx-Yu. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cobalt;

namespace WopiCobaltHost
{
    public class CobaltHostLockingStore : HostLockingStore
    {
        private CobaltSession m_session;

        public CobaltHostLockingStore(CobaltSession session)
            : base()
        {
            m_session = session;
        }

        public override WhoAmIRequest.OutputType HandleWhoAmI(WhoAmIRequest.InputType input)
        {
            WhoAmIRequest.OutputType result = new WhoAmIRequest.OutputType();
            result.UserEmailAddress = m_session.Email;
            result.UserIsAnonymous = m_session.IsAnonymous;
            result.UserLogin = m_session.Login;
            result.UserName = m_session.Name;

            return result;
        }

        public override ServerTimeRequest.OutputType HandleServerTime(ServerTimeRequest.InputType input)
        {
            ServerTimeRequest.OutputType result = new ServerTimeRequest.OutputType();
            result.ServerTime = DateTime.UtcNow;

            return result;
        }

        public override LockAndCheckOutStatusRequest.OutputType HandleLockAndCheckOutStatus(LockAndCheckOutStatusRequest.InputType input)
        {
            LockAndCheckOutStatusRequest.OutputType result = new LockAndCheckOutStatusRequest.OutputType();
            result.LockType = 1U;
            result.CheckOutType = 0U;

            return result;
        }

        public override GetExclusiveLockRequest.OutputType HandleGetExclusiveLock(GetExclusiveLockRequest.InputType input)
        {
            GetExclusiveLockRequest.OutputType result = new GetExclusiveLockRequest.OutputType();

            return result;
        }

        public override RefreshExclusiveLockRequest.OutputType HandleRefreshExclusiveLock(RefreshExclusiveLockRequest.InputType input)
        {
            RefreshExclusiveLockRequest.OutputType result = new RefreshExclusiveLockRequest.OutputType();

            return result;
        }

        public override CheckExclusiveLockAvailabilityRequest.OutputType HandleCheckExclusiveLockAvailability(CheckExclusiveLockAvailabilityRequest.InputType input)
        {
            CheckExclusiveLockAvailabilityRequest.OutputType result = new CheckExclusiveLockAvailabilityRequest.OutputType();

            return result;
        }

        public override ConvertExclusiveLockToSchemaLockRequest.OutputType HandleConvertExclusiveLockToSchemaLock(ConvertExclusiveLockToSchemaLockRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
        {
            ConvertExclusiveLockToSchemaLockRequest.OutputType result = new ConvertExclusiveLockToSchemaLockRequest.OutputType();

            return result;
        }

        public override ConvertExclusiveLockWithCoauthTransitionRequest.OutputType HandleConvertExclusiveLockWithCoauthTransition(ConvertExclusiveLockWithCoauthTransitionRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
        {
            ConvertExclusiveLockWithCoauthTransitionRequest.OutputType result = new ConvertExclusiveLockWithCoauthTransitionRequest.OutputType();

            return result;
        }

        public override GetSchemaLockRequest.OutputType HandleGetSchemaLock(GetSchemaLockRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
        {
            GetSchemaLockRequest.OutputType result = new GetSchemaLockRequest.OutputType();

            return result;
        }


        public override ReleaseExclusiveLockRequest.OutputType HandleReleaseExclusiveLock(ReleaseExclusiveLockRequest.InputType input)
        {
            ReleaseExclusiveLockRequest.OutputType result = new ReleaseExclusiveLockRequest.OutputType();

            return result;
        }

        public override ReleaseSchemaLockRequest.OutputType HandleReleaseSchemaLock(ReleaseSchemaLockRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
        {
            ReleaseSchemaLockRequest.OutputType result = new ReleaseSchemaLockRequest.OutputType();

            return result;
        }

        public override RefreshSchemaLockRequest.OutputType HandleRefreshSchemaLock(RefreshSchemaLockRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
        {
            RefreshSchemaLockRequest.OutputType result = new RefreshSchemaLockRequest.OutputType();
            result.Lock = LockType.SchemaLock;

            return result;
        }

        public override ConvertSchemaLockToExclusiveLockRequest.OutputType HandleConvertSchemaLockToExclusiveLock(ConvertSchemaLockToExclusiveLockRequest.InputType input)
        {
            ConvertSchemaLockToExclusiveLockRequest.OutputType result = new ConvertSchemaLockToExclusiveLockRequest.OutputType();

            return result;
        }

        public override CheckSchemaLockAvailabilityRequest.OutputType HandleCheckSchemaLockAvailability(CheckSchemaLockAvailabilityRequest.InputType input)
        {
            CheckSchemaLockAvailabilityRequest.OutputType result = new CheckSchemaLockAvailabilityRequest.OutputType();

            return result;
        }

        public override JoinCoauthoringRequest.OutputType HandleJoinCoauthoring(JoinCoauthoringRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
        {
            JoinCoauthoringRequest.OutputType result = new JoinCoauthoringRequest.OutputType();
            result.Lock = LockType.SchemaLock;
            result.CoauthStatus = CoauthStatusType.Alone;
            result.TransitionId = Guid.NewGuid();
            return result;
        }

        public override ExitCoauthoringRequest.OutputType HandleExitCoauthoring(ExitCoauthoringRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
        {
            ExitCoauthoringRequest.OutputType result = new ExitCoauthoringRequest.OutputType();

            return result;
        }

        public override RefreshCoauthoringSessionRequest.OutputType HandleRefreshCoauthoring(RefreshCoauthoringSessionRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
        {
            RefreshCoauthoringSessionRequest.OutputType result = new RefreshCoauthoringSessionRequest.OutputType();
            result.Lock = LockType.SchemaLock;
            result.CoauthStatus = CoauthStatusType.Alone;

            return result;
        }

        public override ConvertCoauthLockToExclusiveLockRequest.OutputType HandleConvertCoauthLockToExclusiveLock(ConvertCoauthLockToExclusiveLockRequest.InputType input)
        {
            ConvertCoauthLockToExclusiveLockRequest.OutputType result = new ConvertCoauthLockToExclusiveLockRequest.OutputType();

            return result;
        }

        public override CheckCoauthLockAvailabilityRequest.OutputType HandleCheckCoauthLockAvailability(CheckCoauthLockAvailabilityRequest.InputType input)
        {
            CheckCoauthLockAvailabilityRequest.OutputType result = new CheckCoauthLockAvailabilityRequest.OutputType();

            return result;
        }

        public override MarkCoauthTransitionCompleteRequest.OutputType HandleMarkCoauthTransitionComplete(MarkCoauthTransitionCompleteRequest.InputType input)
        {
            MarkCoauthTransitionCompleteRequest.OutputType result = new MarkCoauthTransitionCompleteRequest.OutputType();

            return result;
        }

        public override GetCoauthoringStatusRequest.OutputType HandleGetCoauthoringStatus(GetCoauthoringStatusRequest.InputType input)
        {
            GetCoauthoringStatusRequest.OutputType result = new GetCoauthoringStatusRequest.OutputType();
            result.CoauthStatus = CoauthStatusType.Alone;

            return result;
        }

        public override Dictionary<string, EditorsTableEntry> QueryEditorsTable()
        {
            return new Dictionary<string,EditorsTableEntry>();
        }

        public override JoinEditingSessionRequest.OutputType HandleJoinEditingSession(JoinEditingSessionRequest.InputType input)
        {
            JoinEditingSessionRequest.OutputType result = new JoinEditingSessionRequest.OutputType();

            return result;
        }

        public override RefreshEditingSessionRequest.OutputType HandleRefreshEditingSession(RefreshEditingSessionRequest.InputType input)
        {
            RefreshEditingSessionRequest.OutputType result = new RefreshEditingSessionRequest.OutputType();

            return result;
        }

        public override LeaveEditingSessionRequest.OutputType HandleLeaveEditingSession(LeaveEditingSessionRequest.InputType input)
        {
            LeaveEditingSessionRequest.OutputType result = new LeaveEditingSessionRequest.OutputType();

            return result;
        }

        public override UpdateEditorMetadataRequest.OutputType HandleUpdateEditorMetadata(UpdateEditorMetadataRequest.InputType input)
        {
            UpdateEditorMetadataRequest.OutputType result = new UpdateEditorMetadataRequest.OutputType();

            return result;
        }

        public override RemoveEditorMetadataRequest.OutputType HandleRemoveEditorMetadata(RemoveEditorMetadataRequest.InputType input)
        {
            RemoveEditorMetadataRequest.OutputType result = new RemoveEditorMetadataRequest.OutputType();

            return result;
        }

        public override ulong GetEditorsTableWaterline()
        {
            return 0;
        }

        public override AmIAloneRequest.OutputType HandleAmIAlone(AmIAloneRequest.InputType input)
        {
            AmIAloneRequest.OutputType result = new AmIAloneRequest.OutputType();
            result.AmIAlone = true;

            return result;
        }

        public override DocMetaInfoRequest.OutputType HandleDocMetaInfo(DocMetaInfoRequest.InputType input)
        {
            DocMetaInfoRequest.OutputType result = new DocMetaInfoRequest.OutputType();

            return result;
        }

        public override VersionsRequest.OutputType HandleVersions(VersionsRequest.InputType input)
        {
            VersionsRequest.OutputType result = new VersionsRequest.OutputType();
            result.Enabled = false;

            return result;
        }
    }
}
