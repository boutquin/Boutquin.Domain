#!/usr/bin/env python3
"""Verify the immutable Domain 1.0.1 family archives before uploading them."""
import hashlib
import json
from pathlib import Path
import sys
import xml.etree.ElementTree as ET
import zipfile

SOURCE = '493b14ceccdbdec2880205961f3aea09d0100322'
ARCHIVES = {'Boutquin.AspNetCore.1.0.1.nupkg': '9682a1e0733e9a84f439bebe614bc239909f41e1bea8b6e7029d3fdcc51756d8', 'Boutquin.AspNetCore.1.0.1.snupkg': '17ef0e1a4b904d5fe495cad1bdf9cc9b6dbc3babfcecd3b322f72e64a1262e09', 'Boutquin.Domain.1.0.1.nupkg': 'beb0ea157399e0e793e424c500ca72083cc611ed627718bfc3c602553ab7c14b', 'Boutquin.Domain.1.0.1.snupkg': 'eeb3d31eeb056454875409d5215e03847b977eff2719f4e5b4c17ec72ddd378c', 'Boutquin.Validation.1.0.1.nupkg': '65be0fd85fcec4b0238de4726bf1debcd4bd8c5c0e94cd2c40a0e9cf56b2412f', 'Boutquin.Validation.1.0.1.snupkg': '11fcda926803f2bfdef80f1046c9eee9b6513b5e99863f91b5ea0894ee158c81'}


def verify(directory, commit):
    if commit != SOURCE:
        raise ValueError("Release tag does not name the qualified source")
    if directory.is_symlink() or {p.name for p in directory.iterdir()} != set(ARCHIVES):
        raise ValueError("Release archive inventory differs")
    for name, expected in ARCHIVES.items():
        path = directory / name
        if path.is_symlink() or not path.is_file():
            raise ValueError("Release archive is not a regular file")
        if hashlib.sha256(path.read_bytes()).hexdigest() != expected:
            raise ValueError("Qualified archive bytes changed: " + name)
        with zipfile.ZipFile(path) as archive:
            package_id = name.split(".1.0.1.")[0]
            metadata = ET.fromstring(archive.read(package_id + ".nuspec"))
            values = {node.tag.rsplit("}", 1)[-1]: node for node in metadata.iter()}
            if values["id"].text != package_id or values["version"].text != "1.0.1":
                raise ValueError("Package identity differs")
            if values["repository"].attrib.get("commit") != SOURCE:
                raise ValueError("Package source differs")
    return {"status": "PASS_EXACT_QUALIFIED_ARCHIVES", "source": SOURCE, "sha256": ARCHIVES}


if __name__ == "__main__":
    print(json.dumps(verify(Path(sys.argv[1]), sys.argv[2]), indent=2))
