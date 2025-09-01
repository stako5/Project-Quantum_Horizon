#!/usr/bin/env python3
import json, os, textwrap

ROOT = os.path.dirname(__file__)
DATA = os.path.join(ROOT, 'data', 'bosses.json')
OUT = os.path.join(ROOT, 'STORIES.md')

def main():
    with open(DATA, 'r') as f:
        data = json.load(f)
    bosses = data.get('bosses', [])
    lines = []
    lines.append('# Mythic Bosses — Stories\n')
    for b in bosses:
        lines.append(f"## {b['name']} ({b['biome']})\n")
        desc = b.get('description','').strip()
        wrapped = '\n'.join(textwrap.wrap(desc, width=100))
        lines.append(wrapped + '\n')
        mechs = ', '.join(b.get('mechanics', []))
        puzzles = ', '.join(b.get('puzzle_modules', []))
        phases = ', '.join(b.get('phases', []))
        lines.append(f"- Mechanics: {mechs}")
        lines.append(f"- Puzzles: {puzzles}")
        lines.append(f"- Phases: {phases}\n")
    with open(OUT, 'w') as f:
        f.write('\n'.join(lines))
    print(f"Wrote {OUT}")

if __name__ == '__main__':
    main()

