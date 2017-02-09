using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SlidingBots
{
    public enum Color
    {
        Red = 0,
        Blue = 1,
        Green = 2,
        Yellow = 3
    }

    public class Field
    {
        public void Initialize()
        {
            Console.WriteLine("Start");

            verticalWall[0] = new uint[] { 4, 10, MapSize };
            verticalWall[1] = new uint[] { 6, 14, MapSize };
            verticalWall[2] = new uint[] { MapSize };
            verticalWall[3] = new uint[] { 2, 9, MapSize };
            verticalWall[4] = new uint[] { 5, MapSize };
            verticalWall[5] = new uint[] { 3, 8, MapSize };
            verticalWall[6] = new uint[] { 11, 14, MapSize };
            verticalWall[7] = new uint[] { 7, 9, MapSize };
            verticalWall[8] = new uint[] { 5, 7, 9, MapSize };
            verticalWall[9] = new uint[] { 2, 13, MapSize };
            verticalWall[10] = new uint[] { MapSize };
            verticalWall[11] = new uint[] { 10, MapSize };
            verticalWall[12] = new uint[] { MapSize };
            verticalWall[13] = new uint[] { 5, 15, MapSize };
            verticalWall[14] = new uint[] { 2, 10, MapSize };
            verticalWall[15] = new uint[] { 6, 12, MapSize };
            horizontalWall[0] = new uint[] { 7, 12, MapSize };
            horizontalWall[1] = new uint[] { 3, 15, MapSize };
            horizontalWall[2] = new uint[] { 6, 9, MapSize };
            horizontalWall[3] = new uint[] { MapSize };
            horizontalWall[4] = new uint[] { 13, MapSize };
            horizontalWall[5] = new uint[] { 4, 9, MapSize };
            horizontalWall[6] = new uint[] { 2, MapSize };
            horizontalWall[7] = new uint[] { 6, 7, 9, MapSize };
            horizontalWall[8] = new uint[] { 7, 9, MapSize };
            horizontalWall[9] = new uint[] { 3, 12, MapSize };
            horizontalWall[10] = new uint[] { 7, 14, MapSize };
            horizontalWall[11] = new uint[] { MapSize };
            horizontalWall[12] = new uint[] { MapSize };
            horizontalWall[13] = new uint[] { 1, 10, MapSize };
            horizontalWall[14] = new uint[] { 7, 13, MapSize };
            horizontalWall[15] = new uint[] { 5, 12, MapSize };

            int prob = 15;

            switch (prob)
            {
                case 1:
                    botId = toBotId(
                        15, 4,
                        10, 13,
                        2, 8,
                        12, 10
                    );
                    setGoal(6, 1, Color.Blue);
                    break;
                case 2:
                    botId = toBotId(
                        15, 4,
                        6, 1,
                        2, 8,
                        12, 0
                    );
                    setGoal(5, 8, Color.Yellow);
                    break;
                case 8:
                    botId = toBotId(
                        2, 5,
                        4, 13,
                        5, 15,
                        5, 4
                    );
                    setGoal(9, 11, Color.Yellow);
                    break;
                case 15:
                    botId = toBotId(
                        14, 13,
                        0, 11,
                        9, 12,
                        6, 15
                    );
                    setGoal(1, 3, Color.Yellow);
                    break;
                case 16:
                    botId = toBotId(
                        14, 13,
                        0, 2,
                        9, 12,
                        1, 3
                    );
                    setGoal(13, 1, Color.Green);
                    break; 
            }

            UnityEngine.Debug.Log(botId.ToString("X"));

            count = 0;
            var tick = -Environment.TickCount;
            iddfs(botId);
            tick += Environment.TickCount;
            UnityEngine.Debug.Log("End");
            UnityEngine.Debug.Log("Time: " + (double)tick / 1000);
        }

        static uint toBotId(
            uint redX, uint redY,
            uint blueX, uint blueY,
            uint greenX, uint greenY,
            uint yellowX, uint yellowY
        )
        {
            return ((((((((((((((redX << 4) + redY) << 4)
            + blueX) << 4) + blueY) << 4)
            + greenX) << 4) + greenY) << 4)
            + yellowX) << 4) + yellowY);
        }

        static uint getPlusPos(
            uint current,
            uint[] wall,
            uint bot1,
            uint bot2,
            uint bot3)
        {
            uint ret = MapSize;
            if (bot1 > current && bot1 < ret)
            {
                ret = bot1;
            }
            if (bot2 > current && bot2 < ret)
            {
                ret = bot2;
            }
            if (bot3 > current && bot3 < ret)
            {
                ret = bot3;
            }
            foreach (uint val in wall)
            {
                if (val > current && val < ret)
                {
                    ret = val;
                }
            }

            return ret - 1;
        }

        static uint getMinusPos(
            uint current,
            uint[] wall,
            uint bot1,
            uint bot2,
            uint bot3)
        {
            uint ret = 0;
            if (bot1 <= current && bot1 >= ret)
            {
                ret = bot1 + 1;
            }
            if (bot2 <= current && bot2 >= ret)
            {
                ret = bot2 + 1;
            }
            if (bot3 <= current && bot3 >= ret)
            {
                ret = bot3 + 1;
            }
            for (int i = 0, wallLength = wall.Length; i < wallLength; i++)
            {
                uint val = wall[i];
                if (val <= current && val > ret)
                {
                    ret = val;
                }
            }

            return ret;
        }

        bool isGoal(uint bots)
        {
            return (bots & goalMask) == goal;
        }

        IEnumerable<uint> expand(uint bots)
        {
            uint redX = bots >> 28;
            uint redY = (bots >> 24) & 0xf;
            uint blueX = (bots >> 20) & 0xf;
            uint blueY = (bots >> 16) & 0xf;
            uint greenX = (bots >> 12) & 0xf;
            uint greenY = (bots >> 8) & 0xf;
            uint yellowX = (bots >> 4) & 0xf;
            uint yellowY = bots & 0xf;

            // yellow right
            {
                uint next = getPlusPos(yellowX,
                                verticalWall[yellowY],
                                redY == yellowY ? redX : MapSize,
                                blueY == yellowY ? blueX : MapSize,
                                greenY == yellowY ? greenX : MapSize
                            );
                if (next != yellowX)
                {
                    yield return toBotId(redX, redY, blueX, blueY, greenX, greenY, next, yellowY);
                }
            }

            // yellow left
            {
                uint next = getMinusPos(yellowX,
                                verticalWall[yellowY],
                                redY == yellowY ? redX : MapSize,
                                blueY == yellowY ? blueX : MapSize,
                                greenY == yellowY ? greenX : MapSize
                            );
                if (next != yellowX)
                {
                    yield return toBotId(redX, redY, blueX, blueY, greenX, greenY, next, yellowY);
                }
            }

            // yellow down
            {
                uint next = getPlusPos(yellowY,
                                horizontalWall[yellowX],
                                redX == yellowX ? redY : MapSize,
                                blueX == yellowX ? blueY : MapSize,
                                greenX == yellowX ? greenY : MapSize
                            );
                if (next != yellowY)
                {
                    yield return toBotId(redX, redY, blueX, blueY, greenX, greenY, yellowX, next);
                }
            }

            // yellow up
            {
                uint next = getMinusPos(yellowY,
                                horizontalWall[yellowX],
                                redX == yellowX ? redY : MapSize,
                                blueX == yellowX ? blueY : MapSize,
                                greenX == yellowX ? greenY : MapSize
                            );
                if (next != yellowY)
                {
                    yield return toBotId(redX, redY, blueX, blueY, greenX, greenY, yellowX, next);
                }
            }

            // red right
            {
                uint next = getPlusPos(redX,
                                verticalWall[redY],
                                blueY == redY ? blueX : MapSize,
                                greenY == redY ? greenX : MapSize,
                                yellowY == redY ? yellowX : MapSize
                            );
                if (next != redX)
                {
                    yield return toBotId(next, redY, blueX, blueY, greenX, greenY, yellowX, yellowY);
                }
            }

            // red left
            {
                uint next = getMinusPos(redX,
                                verticalWall[redY],
                                blueY == redY ? blueX : MapSize,
                                greenY == redY ? greenX : MapSize,
                                yellowY == redY ? yellowX : MapSize
                            );
                if (next != redX)
                {
                    yield return toBotId(next, redY, blueX, blueY, greenX, greenY, yellowX, yellowY);
                }
            }

            // red down
            {
                uint next = getPlusPos(redY,
                                horizontalWall[redX],
                                blueX == redX ? blueY : MapSize,
                                greenX == redX ? greenY : MapSize,
                                yellowX == redX ? yellowY : MapSize
                            );
                if (next != redY)
                {
                    yield return toBotId(redX, next, blueX, blueY, greenX, greenY, yellowX, yellowY);
                }
            }

            // red up
            {
                uint next = getMinusPos(redY,
                                horizontalWall[redX],
                                blueX == redX ? blueY : MapSize,
                                greenX == redX ? greenY : MapSize,
                                yellowX == redX ? yellowY : MapSize
                            );
                if (next != redY)
                {
                    yield return toBotId(redX, next, blueX, blueY, greenX, greenY, yellowX, yellowY);
                }
            }
            // blue right
            {
                uint next = getPlusPos(blueX,
                                verticalWall[blueY],
                                redY == blueY ? redX : MapSize,
                                greenY == blueY ? greenX : MapSize,
                                yellowY == blueY ? yellowX : MapSize
                            );
                if (next != blueX)
                {
                    yield return toBotId(redX, redY, next, blueY, greenX, greenY, yellowX, yellowY);
                }
            }

            // blue left
            {
                uint next = getMinusPos(blueX,
                                verticalWall[blueY],
                                redY == blueY ? redX : MapSize,
                                greenY == blueY ? greenX : MapSize,
                                yellowY == blueY ? yellowX : MapSize
                            );
                if (next != blueX)
                {
                    yield return toBotId(redX, redY, next, blueY, greenX, greenY, yellowX, yellowY);
                }
            }

            // blue down
            {
                uint next = getPlusPos(blueY,
                                horizontalWall[blueX],
                                redX == blueX ? redY : MapSize,
                                greenX == blueX ? greenY : MapSize,
                                yellowX == blueX ? yellowY : MapSize
                            );
                if (next != blueY)
                {
                    yield return toBotId(redX, redY, blueX, next, greenX, greenY, yellowX, yellowY);
                }
            }

            // blue up
            {
                uint next = getMinusPos(blueY,
                                horizontalWall[blueX],
                                redX == blueX ? redY : MapSize,
                                greenX == blueX ? greenY : MapSize,
                                yellowX == blueX ? yellowY : MapSize
                            );
                if (next != blueY)
                {
                    yield return toBotId(redX, redY, blueX, next, greenX, greenY, yellowX, yellowY);
                }
            }
            // green right
            {
                uint next = getPlusPos(greenX,
                                verticalWall[greenY],
                                redY == greenY ? redX : MapSize,
                                blueY == greenY ? blueX : MapSize,
                                yellowY == greenY ? yellowX : MapSize
                            );
                if (next != greenX)
                {
                    yield return toBotId(redX, redY, blueX, blueY, next, greenY, yellowX, yellowY);
                }
            }

            // green left
            {
                uint next = getMinusPos(greenX,
                                verticalWall[greenY],
                                redY == greenY ? redX : MapSize,
                                blueY == greenY ? blueX : MapSize,
                                yellowY == greenY ? yellowX : MapSize
                            );
                if (next != greenX)
                {
                    yield return toBotId(redX, redY, blueX, blueY, next, greenY, yellowX, yellowY);
                }
            }

            // green down
            {
                uint next = getPlusPos(greenY,
                                horizontalWall[greenX],
                                redX == greenX ? redY : MapSize,
                                blueX == greenX ? blueY : MapSize,
                                yellowX == greenX ? yellowY : MapSize
                            );
                if (next != greenY)
                {
                    yield return toBotId(redX, redY, blueX, blueY, greenX, next, yellowX, yellowY);
                }
            }

            // green up
            {
                uint next = getMinusPos(greenY,
                                horizontalWall[greenX],
                                redX == greenX ? redY : MapSize,
                                blueX == greenX ? blueY : MapSize,
                                yellowX == greenX ? yellowY : MapSize
                            );
                if (next != greenY)
                {
                    yield return toBotId(redX, redY, blueX, blueY, greenX, next, yellowX, yellowY);
                }
            }


        }

        IEnumerable<uint> expand(uint bots, Color color)
        {
            uint redX = bots >> 28;
            uint redY = (bots >> 24) & 0xf;
            uint blueX = (bots >> 20) & 0xf;
            uint blueY = (bots >> 16) & 0xf;
            uint greenX = (bots >> 12) & 0xf;
            uint greenY = (bots >> 8) & 0xf;
            uint yellowX = (bots >> 4) & 0xf;
            uint yellowY = bots & 0xf;

            switch (color)
            {
                case Color.Red:
                    // red right
                    {
                        uint next = getPlusPos(redX,
                                        verticalWall[redY],
                                        blueY == redY ? blueX : MapSize,
                                        greenY == redY ? greenX : MapSize,
                                        yellowY == redY ? yellowX : MapSize
                                    );
                        if (next != redX)
                        {
                            yield return toBotId(next, redY, blueX, blueY, greenX, greenY, yellowX, yellowY);
                        }
                    }

                    // red left
                    {
                        uint next = getMinusPos(redX,
                                        verticalWall[redY],
                                        blueY == redY ? blueX : MapSize,
                                        greenY == redY ? greenX : MapSize,
                                        yellowY == redY ? yellowX : MapSize
                                    );
                        if (next != redX)
                        {
                            yield return toBotId(next, redY, blueX, blueY, greenX, greenY, yellowX, yellowY);
                        }
                    }

                    // red down
                    {
                        uint next = getPlusPos(redY,
                                        horizontalWall[redX],
                                        blueX == redX ? blueY : MapSize,
                                        greenX == redX ? greenY : MapSize,
                                        yellowX == redX ? yellowY : MapSize
                                    );
                        if (next != redY)
                        {
                            yield return toBotId(redX, next, blueX, blueY, greenX, greenY, yellowX, yellowY);
                        }
                    }

                    // red up
                    {
                        uint next = getMinusPos(redY,
                                        horizontalWall[redX],
                                        blueX == redX ? blueY : MapSize,
                                        greenX == redX ? greenY : MapSize,
                                        yellowX == redX ? yellowY : MapSize
                                    );
                        if (next != redY)
                        {
                            yield return toBotId(redX, next, blueX, blueY, greenX, greenY, yellowX, yellowY);
                        }
                    }
                    break;
                case Color.Blue:
                    // blue right
                    {
                        uint next = getPlusPos(blueX,
                                        verticalWall[blueY],
                                        redY == blueY ? redX : MapSize,
                                        greenY == blueY ? greenX : MapSize,
                                        yellowY == blueY ? yellowX : MapSize
                                    );
                        if (next != blueX)
                        {
                            yield return toBotId(redX, redY, next, blueY, greenX, greenY, yellowX, yellowY);
                        }
                    }

                    // blue left
                    {
                        uint next = getMinusPos(blueX,
                                        verticalWall[blueY],
                                        redY == blueY ? redX : MapSize,
                                        greenY == blueY ? greenX : MapSize,
                                        yellowY == blueY ? yellowX : MapSize
                                    );
                        if (next != blueX)
                        {
                            yield return toBotId(redX, redY, next, blueY, greenX, greenY, yellowX, yellowY);
                        }
                    }

                    // blue down
                    {
                        uint next = getPlusPos(blueY,
                                        horizontalWall[blueX],
                                        redX == blueX ? redY : MapSize,
                                        greenX == blueX ? greenY : MapSize,
                                        yellowX == blueX ? yellowY : MapSize
                                    );
                        if (next != blueY)
                        {
                            yield return toBotId(redX, redY, blueX, next, greenX, greenY, yellowX, yellowY);
                        }
                    }

                    // blue up
                    {
                        uint next = getMinusPos(blueY,
                                        horizontalWall[blueX],
                                        redX == blueX ? redY : MapSize,
                                        greenX == blueX ? greenY : MapSize,
                                        yellowX == blueX ? yellowY : MapSize
                                    );
                        if (next != blueY)
                        {
                            yield return toBotId(redX, redY, blueX, next, greenX, greenY, yellowX, yellowY);
                        }
                    }
                    
                    break;
                case Color.Green:
                    // green right
                    {
                        uint next = getPlusPos(greenX,
                                        verticalWall[greenY],
                                        redY == greenY ? redX : MapSize,
                                        blueY == greenY ? blueX : MapSize,
                                        yellowY == greenY ? yellowX : MapSize
                                    );
                        if (next != greenX)
                        {
                            yield return toBotId(redX, redY, blueX, blueY, next, greenY, yellowX, yellowY);
                        }
                    }

                    // green left
                    {
                        uint next = getMinusPos(greenX,
                                        verticalWall[greenY],
                                        redY == greenY ? redX : MapSize,
                                        blueY == greenY ? blueX : MapSize,
                                        yellowY == greenY ? yellowX : MapSize
                                    );
                        if (next != greenX)
                        {
                            yield return toBotId(redX, redY, blueX, blueY, next, greenY, yellowX, yellowY);
                        }
                    }

                    // green down
                    {
                        uint next = getPlusPos(greenY,
                                        horizontalWall[greenX],
                                        redX == greenX ? redY : MapSize,
                                        blueX == greenX ? blueY : MapSize,
                                        yellowX == greenX ? yellowY : MapSize
                                    );
                        if (next != greenY)
                        {
                            yield return toBotId(redX, redY, blueX, blueY, greenX, next, yellowX, yellowY);
                        }
                    }

                    // green up
                    {
                        uint next = getMinusPos(greenY,
                                        horizontalWall[greenX],
                                        redX == greenX ? redY : MapSize,
                                        blueX == greenX ? blueY : MapSize,
                                        yellowX == greenX ? yellowY : MapSize
                                    );
                        if (next != greenY)
                        {
                            yield return toBotId(redX, redY, blueX, blueY, greenX, next, yellowX, yellowY);
                        }
                    }
                    break;
                case Color.Yellow:
                    // yellow right
                    {
                        uint next = getPlusPos(yellowX,
                                        verticalWall[yellowY],
                                        redY == yellowY ? redX : MapSize,
                                        blueY == yellowY ? blueX : MapSize,
                                        greenY == yellowY ? greenX : MapSize
                                    );
                        if (next != yellowX)
                        {
                            yield return toBotId(redX, redY, blueX, blueY, greenX, greenY, next, yellowY);
                        }
                    }

                    // yellow left
                    {
                        uint next = getMinusPos(yellowX,
                                        verticalWall[yellowY],
                                        redY == yellowY ? redX : MapSize,
                                        blueY == yellowY ? blueX : MapSize,
                                        greenY == yellowY ? greenX : MapSize
                                    );
                        if (next != yellowX)
                        {
                            yield return toBotId(redX, redY, blueX, blueY, greenX, greenY, next, yellowY);
                        }
                    }

                    // yellow down
                    {
                        uint next = getPlusPos(yellowY,
                                        horizontalWall[yellowX],
                                        redX == yellowX ? redY : MapSize,
                                        blueX == yellowX ? blueY : MapSize,
                                        greenX == yellowX ? greenY : MapSize
                                    );
                        if (next != yellowY)
                        {
                            yield return toBotId(redX, redY, blueX, blueY, greenX, greenY, yellowX, next);
                        }
                    }

                    // yellow up
                    {
                        uint next = getMinusPos(yellowY,
                                        horizontalWall[yellowX],
                                        redX == yellowX ? redY : MapSize,
                                        blueX == yellowX ? blueY : MapSize,
                                        greenX == yellowX ? greenY : MapSize
                                    );
                        if (next != yellowY)
                        {
                            yield return toBotId(redX, redY, blueX, blueY, greenX, greenY, yellowX, next);
                        }
                    }
                    break; 
            } 
        }

        uint iddfs(uint bots)
        {
            for (int depth = 0;; ++depth)
            {
                var tick = -Environment.TickCount;
                var found = dls(bots, depth, 0);
                tick += Environment.TickCount;
                UnityEngine.Debug.Log("depth: " + depth);
                UnityEngine.Debug.Log("time: " + (double)tick / 1000);

                if (found != null)
                {
                    UnityEngine.Debug.Log(bots.ToString("X"));
                    UnityEngine.Debug.Log("search: " + count);
                    UnityEngine.Debug.Log("Depth: " + depth);
                    return (uint)found;
                }
            }
        }

        uint? dls(uint bots, int depth, uint prev)
        {
            ++count;
            if (isGoal(bots))
            {
                return bots;
            }
            if (depth == 1)
            {
                
                if (!(((bots - goal) & GoalMaskX) == 0 || ((bots - goal) & goalMaskY) == 0))
                {
                    return null;
                }
                foreach (var child in expand(bots, goalColor))
                {
                    if (child == prev)
                    {
                        continue;
                    }

                    var found = dls(child, depth - 1, bots);
                    if (found != null)
                    {
                        UnityEngine.Debug.Log(child.ToString("X"));
                        return found;
                    }
                }
            }

            if (depth > 0)
            {
                foreach (var child in expand(bots))
                {
                    if (child == prev)
                    {
                        continue;
                    }

                    var found = dls(child, depth - 1, bots);
                    if (found != null)
                    {
                        UnityEngine.Debug.Log(child.ToString("X"));
                        return found;
                    }
                }
            }
            return null;
        }

        void setGoal(uint x, uint y, Color color)
        {
            goal = ((x << 4) + y) << (3 - (int)color) * 8;
            goalMask = (uint)0x000000ff << (3 - (int)color) * 8;
            GoalMaskX = (uint)0x000000f0 << (3 - (int)color) * 8;
            goalMaskY = (uint)0x0000000f << (3 - (int)color) * 8;
            goalColor = color;
        }

        // red blue green yellow
        uint botId;
        uint goal;
        Color goalColor;
        uint goalMask;
        uint GoalMaskX;
        uint goalMaskY;
        int count;
        uint[][] verticalWall = new uint[MapSize][];
        uint[][] horizontalWall = new uint[MapSize][];
        const uint MapSize = 16;
    }
}