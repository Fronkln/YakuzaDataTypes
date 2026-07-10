using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Yarhl.IO;

namespace YakuzaDataTypes.PAC
{
    internal static class PACImporter
    {
        public static PAC Read(string path, Game game)
        {
            return Read(File.ReadAllBytes(path), game);
        }

        public static PAC Read(byte[] fileBuffer, Game game)
        {
            PAC pac = new PAC();

            var m_readStream = DataStreamFactory.FromArray(fileBuffer, 0, fileBuffer.Length);
            var m_reader = new DataReader(m_readStream) { DefaultEncoding = Encoding.GetEncoding(932), Endianness = EndiannessMode.BigEndian };

            int nodeCount = m_reader.ReadInt16();
            int fileType = m_reader.ReadInt16();
            int nodesStart = m_reader.ReadInt32();

            if (fileType != 0)
                throw new System.Exception("Cant read pac type " + fileType);

            var pacEntities = new PACEntityY5[nodeCount];
            Y5PacNodeHeader[] nodeHeaders = new Y5PacNodeHeader[nodeCount];

            for (int i = 0; i < nodeCount; i++)
            {
                var header = new Y5PacNodeHeader();

                header.NodeID = m_reader.ReadInt32();
                header.Type = (byte)((header.NodeID >> 24) & 0xFF);
                header.CCCDataPtr = m_reader.ReadInt32();
                header.EntityDataPtr = m_reader.ReadInt32();
                header.Data1Size = m_reader.ReadInt16();
                header.Data2Size = m_reader.ReadInt16();

                nodeHeaders[i] = header;
            }

            for (int i = 0; i < nodeHeaders.Length; i++)
            {
                var header = nodeHeaders[i];

                m_reader.Stream.Position = header.EntityDataPtr;

                int totalData2Count = 0;
                m_reader.Stream.RunInPosition(delegate { totalData2Count = m_reader.ReadByte(); }, 14, SeekMode.Current);
                int sizePerData = header.Data2Size / totalData2Count;


                BasePACEntityDataY5[] entityDats = new BasePACEntityDataY5[totalData2Count];

                for (int k = 0; k < totalData2Count; k++)
                {
                    entityDats[k] = BasePACEntityDataY5.Read(m_reader, header.Type, sizePerData);
                }

                m_reader.Stream.Position = header.CCCDataPtr;
                PACEntityMsgDataY5 entityMsgData = new PACEntityMsgDataY5();

                if (header.CCCDataPtr > 0)
                {
                    long msgStart = m_reader.Stream.Position;

                    //literally msg header
                    PACCCCHeaderY5 msgHeader = new PACCCCHeaderY5();
                    msgHeader.Identifier = m_reader.ReadBytes(3);
                    msgHeader.GroupsCount = m_reader.ReadByte();
                    msgHeader.GroupsPtr = m_reader.ReadInt32();
                    msgHeader.PositionsPtr = m_reader.ReadInt32();
                    msgHeader.PositionsCount = m_reader.ReadUInt16();
                    msgHeader.StringCount = m_reader.ReadUInt16();
                    msgHeader.StringTablePtr = m_reader.ReadInt32();
                    msgHeader.Unk1Ptr = m_reader.ReadInt32();

                    entityMsgData = new PACEntityMsgDataY5();
                    entityMsgData.Positions = new List<MSG.MsgPositionData>();
                    entityMsgData.Groups = new List<PACMsgGroup>();
                    entityMsgData.Identifier = msgHeader.Identifier;


                    if (msgHeader.GroupsCount > 0)
                    {
                        m_reader.Stream.Seek(msgStart + msgHeader.GroupsPtr);

                        for (int k = 0; k < msgHeader.GroupsCount; k++)
                        {
                            long groupStart = m_reader.Stream.Position;

                            PACMsgGroup group = new PACMsgGroup();
                            int conditionPointer = m_reader.ReadInt32();
                            int refDataPointer = m_reader.ReadInt32();
                            int conditionCount = m_reader.ReadByte();
                            int refDataCount = m_reader.ReadByte();
                            group.Unknown1 = m_reader.ReadByte();
                            group.Unknown2 = m_reader.ReadByte();

                            group.InteractionParameters = m_reader.ReadInt32();

                            long groupEnd = m_reader.Stream.Position;

                            if (conditionCount > 0)
                            {
                                long conditionPos = msgStart + conditionPointer;

                                m_reader.Stream.RunInPosition(delegate
                                {
                                    for (int j = 0; j < conditionCount; j++)
                                    {
                                        PACConditionChunkY5 cond = new PACConditionChunkY5();
                                        cond.Type = m_reader.ReadInt32();
                                        cond.Unknown2 = m_reader.ReadInt32();
                                        cond.Unknown3 = m_reader.ReadInt32();

                                        group.Conditions.Add(cond);
                                    }
                                }, conditionPos);
                            }

                            if (refDataPointer != 0)
                            {
                                group.Refs = new PACRef[refDataCount];

                                long refPos = header.CCCDataPtr + refDataPointer;
                                m_reader.Stream.RunInPosition(delegate
                                {
                                    for (int p = 0; p < refDataCount; p++)
                                    {
                                        PACRef pacRef = new PACRef();

                                        pacRef.TextToggle = m_reader.ReadInt16();
                                        byte refStructCount = m_reader.ReadByte();
                                        m_reader.Stream.Position += 1;

                                        int textPointer = m_reader.ReadInt32();
                                        int refStructPointer = m_reader.ReadInt32();


                                        long textAddr = msgStart + textPointer;

                                        m_reader.Stream.RunInPosition(delegate
                                        {
                                            pacRef.Text = m_reader.ReadString();
                                        }, textAddr);

                                        if (refStructCount > 0)
                                        {
                                            m_reader.Stream.RunInPosition(delegate
                                            {
                                                for (int j = 0; j < refStructCount; j++)
                                                {
                                                    MsgProperty chunk = new MsgProperty();
                                                    chunk.PropertyID = m_reader.ReadUInt16();
                                                    chunk.Unknown = m_reader.ReadUInt16();
                                                    chunk.Unknown2 = m_reader.ReadInt32();
                                                    chunk.Unknown3 = m_reader.ReadInt32();
                                                    chunk.Unknown4 = m_reader.ReadInt32();

                                                    pacRef.MsgProperties.Add(chunk);
                                                }
                                            }, header.CCCDataPtr + refStructPointer);
                                        }

                                        group.Refs[p] = pacRef;
                                    }

                                }, refPos);

                            }

                            m_reader.Stream.Position = groupEnd;
                            entityMsgData.Groups.Add(group);
                        }
                    }

                    if (msgHeader.PositionsCount > 0)
                    {
                        m_reader.Stream.Seek(msgStart + msgHeader.PositionsPtr);

                        for (int k = 0; k < msgHeader.PositionsCount; k++)
                        {
                            MSG.MsgPositionData pos = new MSG.MsgPositionData();
                            pos.Position = m_reader.ReadPXDVector3();
                            pos.Angle = m_reader.ReadInt32();

                            entityMsgData.Positions.Add(pos);
                        }
                    }


                    if (msgHeader.StringCount > 0)
                    {
                        m_reader.Stream.Seek(msgStart + msgHeader.StringTablePtr);
                        int[] stringPtrs = new int[msgHeader.StringCount];

                        for (int k = 0; k < stringPtrs.Length; k++)
                            stringPtrs[k] = m_reader.ReadInt32();

                        string[] strings = new string[stringPtrs.Length];

                        for (int k = 0; k < strings.Length; k++)
                        {
                            m_reader.Stream.Position = msgStart + stringPtrs[k];
                            strings[k] = m_reader.ReadString();
                        }

                        entityMsgData.Strings = strings;

                    }

                    PACEntityY5 entity = new PACEntityY5();
                    entity.UID = header.NodeID;
                    entity.Type = (PACEntityTypeY5)header.Type;
                    entity.EntityData = entityDats;
                    entity.MsgData = entityMsgData;

                    pacEntities[i] = entity;
                }

                m_reader.Stream.Seek(nodesStart, SeekMode.Start);

            }

            return pac;
        }
    }
}
