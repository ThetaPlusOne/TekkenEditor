using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TekkenEditor
{
    public static class SaveConstant
    {
        public const string KEY_STRING = "R1b#:e?%ItKi&fsT?qZJ9nyYv8sNmQ@c";
        public const int SALT_SIZE = 0x4;

        public const int CHKSUM_SIZE = 0x4;
        public const int PADDING_SIZE = 0x8;
        public const int FILE_HEADER_SIZE = 0x10;

        public const int START_OFFSET = SALT_SIZE + CHKSUM_SIZE + FILE_HEADER_SIZE;

        public const int SLOT_COUNTER_OFFSET = 0x2;
        public const int CHARACTER_HEADER_SIZE = 0x2C;


        public const int ITEM_BLOCK_SIZE = 0x20;
        public const int ITEM_ID_SIZE = 0x08;
        public const int ITEM_COLOR_SIZE = 0x04;
        public const int ITEM_FLAG_SIZE = 0x04;



        public const int ITEM_COLOR_NUM = 5;
        public const int ITEM_BLOCK_NUM = 20;

        public const int SLOT_NUM = 10;

        public const int SUM_ITEM_SIZE = ITEM_BLOCK_SIZE * ITEM_BLOCK_NUM * SLOT_NUM;

        public const int MAX_IMAGE_SIZE = 0x19000; //10kB
        public const int IMAGE_META_SIZE = 0x4;

        public const int CHARACTER_BLOCK_SIZE = CHARACTER_HEADER_SIZE + (((ITEM_BLOCK_SIZE * ITEM_BLOCK_NUM) + (MAX_IMAGE_SIZE + IMAGE_META_SIZE)) * SLOT_NUM);

        public const int IMG_WIDTH = 148;
        public const int IMG_HEIGHT = 190;

        public static readonly String[] ITEM_LIST = { "HAT", "HAIR_STYLE", "FULL_FACE", "HAIR_ACC", "GLASSES", "FACE", "MAKEUP", "UNKOWN_FACE", "TOP", "FULL_BODY", "BOTTOM", "UPPER_ACC", "LOWER_ACC", "UNIQUE", "HIT_EFFECT", "AURA_EFFECT", "UNKOWN_ITEM2", "TAN", "MIRROR", "PANEL" };
        public static readonly String[] CHARACTER_LIST = { "Paul", "Law", "King", "Yoshimitsu", "Hwoarang", "Xiaoyu", "JIN", "Byran", "Heihachi", "Kazuya", "Steve", "Jack7", "Asuska", "Devil Jin", "Feng", "Lili", "Dragunov", "Leo", "Lars", "Alisa", "Claudio", "Katarina", "Lucky Chloe", "Shaheen", "Josie", "Gaigas", "Kazumi", "Nina", "Raven", "Lee", "Bob", "Akuma", "Kuma", "Panada", "Eddy", "Eliza", "Miguel" };

    }
}
