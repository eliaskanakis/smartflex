# aberon web services for Smartflex

aberon .net web api providing the following operations:
- Get Picker Details
- Get Picking List
- Picking Confirmation

# End Points

## Get Picker Details

- Method: Get
- Url: api/picker?nameShort=<Picker Login Name>
- Response Sample
```
{
    "pickerId": 1,
    "Name": "Elias Kanakis"
}
```

## Get Picker List

- Method: Get
- Url: api/PickingList/<Picking List Number>
```
{
    "Id": 8532570.0,
    "CusCode": "312043",
    "CusName": "ΓΑΛΑΤΣΙ - ΝΕΑ ΙΩΝΙΑ",
    "Lines": [
        {
            "Id": "0|112916007|2|8532570|1|153|||1387|1|1||",
            "Aisle": "03",
            "Bay": 13,
            "Layer": 0,
            "RelativeBay": 0,
            "ColumnsPerLayer": [
                3
            ],
            "MatrixLayer": 0,
            "MatrixColumn": 0,
            "DistanceFromFloor": 0,
            "DistanceFromMarker": 0,
            "Width": 90,
            "Height": 160,
            "LocAddress": "03.013.0",
            "ItemCode": "730275",
            "ItmDescr": "LIFE ΜΔ ΠΟΡΤΟΚΑΛΙ 1LT",
            "SerialScanRequired": false,
            "IsDual": false,
            "SerialLength": "",
            "SerialPrefix": "",
            "BoxQty": 1.0,
            "Qty": 0.0,
            "TotalQty": 12.0,
            "UnitsPerBox": 12.0,
            "ImgUrl": "http://62.1.171.4:5000/smartflex-delta/730275.jpeg",
            "LutDescr": "ΚΙΒ",
            "ValidBarcodes": []
        }
}
```

## Picking Confirmation

- Method: Post
- Url: api/PickingList
- Request Sample
```
{
    "lineId": "||1|8534070|5|14224|15||2425||||",
    "qty": "4",
    "SerialNumbers":["2912345001231","2912345001121","2912345001111","2912345000761"]
}
```
