﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public static class ObjExtension
    {
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void Log(this object obj, params object[] args)
        {
            if (args?.Length > 0)
                Console.WriteLine(obj.ToString(), args);
            else
                Console.WriteLine(obj);
        }

        public static void ReverseBytes(this byte[] bytes)
        {
            byte tmp;
            int len = bytes.Length;

            for (int i = 0; i < len / 2; i++)
            {
                tmp = bytes[len - 1 - i];
                bytes[len - 1 - i] = bytes[i];
                bytes[i] = tmp;
            }
        }

        public static byte[] GetBigEndBytes(this int obj)
        {
            return BitConverter.GetBytes(obj).Reverse().ToArray();
        }
        
        public static int ReadSmEndInt(this BinaryReader br)
        {
            byte[] buffer = br.ReadBytes(4);
            buffer = buffer.Reverse().ToArray();
            return BitConverter.ToInt32(buffer, 0);
        }
    }

    public class NetMsgHeader
    {
        //public static Logger logger = Logger.getLogger(NetMsgHeader.class.getName());

        private const int FIXED_HEADER_SKIP = 4 + 4 + 4 + 4 + 4;

        public const int CMDID_NOOPING = 6;
        public const int CMDID_NOOPING_RESP = 6;

        public const int CLIENTVERSION = 200;

        public int headLength;
        public int clientVersion;
        public int cmdId;
        public int seq;

        public byte[] options;
        public byte[] body;

        public class InvalidHeaderException : Exception
        {
            public InvalidHeaderException(String message) : base(message)
            {
            }
        }

        /**
         * Decode NetMsgHeader from InputStream
         *
         * @param is close input stream yourself
         * @return
         * @throws IOException
         */
        public bool decode(Stream ins)
        {
            //DataInputStream dis = new DataInputStream(is);
            BinaryReader br = new BinaryReader(ins);
            try
            {
                headLength = br.ReadSmEndInt(); //.ReadInt32(); // dis.readInt();
                clientVersion = br.ReadSmEndInt(); //ReadInt32(); //dis.readInt();
                cmdId = br.ReadSmEndInt(); //ReadInt32(); //dis.readInt();
                seq = br.ReadSmEndInt(); //ReadInt32(); //dis.readInt();
                int bodyLen = br.ReadSmEndInt(); //ReadInt32(); //dis.readInt();

                if (clientVersion != CLIENTVERSION)
                {
                    throw new InvalidHeaderException("invalid client version in header, clientVersion: " + clientVersion + " packlen: " + (headLength + bodyLen));
                }

                //logger.debug(LogUtils.format("dump clientVersion=%d, cmdid=%d, seq=%d, packlen=%d", clientVersion, cmdId, seq, (headLength + bodyLen)));

                // read body?
                if (bodyLen > 0)
                {
                    body = new byte[bodyLen];
                    br.Read(body, 0, bodyLen); //dis.readFully(body);
                }
                else
                {
                    // no body?!
                    switch (cmdId)
                    {
                        case CMDID_NOOPING:
                            break;
                        default:
                            throw new InvalidHeaderException("invalid header body, cmdid:" + cmdId);
                    }
                }
                return true;
            }
            catch (IOException e)
            {
                //e.printStackTrace();
                e.Log();
            }
            return false;
        }
        
        public byte[] encode()
        {
            if (body == null && cmdId != CMDID_NOOPING && cmdId != CMDID_NOOPING_RESP)
            {
                throw new InvalidHeaderException("invalid header body");
            }

            int headerLength = FIXED_HEADER_SKIP + (options == null ? 0 : options.Length);
            int bodyLength = (body == null ? 0 : body.Length);
            int packLength = headerLength + bodyLength;
            //ByteArrayOutputStream baos = new ByteArrayOutputStream(packLength);

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(headerLength.GetBigEndBytes()); //.writeInt(headerLength);
                bw.Write(CLIENTVERSION.GetBigEndBytes()); //writeInt(CLIENTVERSION);
                bw.Write(cmdId.GetBigEndBytes()); //writeInt(cmdId);
                bw.Write(seq.GetBigEndBytes()); //writeInt(seq);
                bw.Write(bodyLength.GetBigEndBytes()); //writeInt(bodyLength);

                if (options != null)
                {
                    bw.Write(options); //dos.write(options);
                }

                if (body != null)
                {
                    bw.Write(body); //dos.write(body);
                }
                return ms.ToArray();
            }

            //try
            //{
            //    DataOutputStream dos = new DataOutputStream(baos);

            //    dos.writeInt(headerLength);
            //    dos.writeInt(CLIENTVERSION);
            //    dos.writeInt(cmdId);
            //    dos.writeInt(seq);
            //    dos.writeInt(bodyLength);

            //    if (options != null)
            //    {
            //        dos.write(options);
            //    }

            //    if (body != null)
            //    {
            //        dos.write(body);
            //    }

            //}
            //catch (IOException e)
            //{
            //    e.printStackTrace();

            //}
            //finally
            //{
            //    try
            //    {
            //        baos.close();

            //    }
            //    catch (IOException e)
            //    {
            //        e.printStackTrace();
            //    }
            //}

            //return baos.toByteArray();
        }
    }
}
